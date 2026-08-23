using TrustedAbstractions;
using TrustedTools;

namespace TrustedCachingRateApi
{
    public sealed class CachingRateApi : IRateApi, IDisposable
    {
        private sealed record CacheEntry(decimal Rate, DateTimeOffset Timestamp);

        private sealed class ConcurrencyGate(string normalCurrencyCode) : IDisposable
        {
            private bool _disposed;
            public int RefCount { get; set; }
            public SemaphoreSlim Semaphore { get; } = new(1, 1);
            public string NormalCurrencyCode { get; } = normalCurrencyCode;

            public void Dispose()
            {
                if (!_disposed)
                {
                    _disposed = true;
                    Semaphore.Dispose();
                }
            }
        }

        private bool _disposed;
        private readonly int _ttlMs;
        private bool _haveCurrencyCodes;
        private readonly IRateApi _realApi;
        private readonly Lock _mainGate = new();
        private readonly TimeProvider _timeProvider;
        private readonly List<string> _currencyCodes = [];
        private readonly SemaphoreSlim _mainSemaphore = new(1, 1);
        private readonly Dictionary<string, CacheEntry> _cache = [];
        private readonly Dictionary<string, ConcurrencyGate> _concurrencyGates = [];

        public CachingRateApi(
            int ttlMs,
            int minTtlMs,
            IRateApi realApi,
            TimeProvider timeProvider)
        {
            ArgumentNullException.ThrowIfNull(realApi);
            ArgumentNullException.ThrowIfNull(timeProvider);
            if (minTtlMs < 1)
                throw new ArgumentOutOfRangeException(nameof(minTtlMs), minTtlMs, "Minimum TTL must be positive.");
            if (ttlMs < minTtlMs)
                throw new ArgumentException($"Invalid ttlMs: {ttlMs}ms", nameof(ttlMs));

            _ttlMs = ttlMs;
            _realApi = realApi;
            _timeProvider = timeProvider;
        }

        public async Task<decimal> GetUsdRate(string currencyCode)
        {
            ThrowIfDisposed();

            var gate = AcquireGate(currencyCode);
            try
            {
                return await GetUsdRateWhileGated(gate);
            }
            finally
            {
                ReleaseGate(gate);
            }
        }

        public async Task<IReadOnlyList<string>> GetSupportedCurrencyCodes()
        {
            ThrowIfDisposed();

            await _mainSemaphore.WaitAsync();

            try
            {
                if (!_haveCurrencyCodes)
                {
                    var currencyCodes = await _realApi.GetSupportedCurrencyCodes();
                    _currencyCodes.AddRange(currencyCodes);
                    _haveCurrencyCodes = true;
                }
                return [.. _currencyCodes];
            }
            finally
            {
                _mainSemaphore.Release();
            }
        }

        private async Task<decimal> GetUsdRateWhileGated(ConcurrencyGate gate)
        {
            await gate.Semaphore.WaitAsync();

            try
            {
                if (TryGetCachedRate(gate, out var rate))
                    return rate;

                rate = await _realApi.GetUsdRate(gate.NormalCurrencyCode);
                StoreCachedRate(gate, rate);
                return rate;
            }
            finally
            {
                gate.Semaphore.Release();
            }
        }

        private bool TryGetCachedRate(ConcurrencyGate gate, out decimal rate)
        {
            lock (_mainGate)
            {
                if (_cache.TryGetValue(gate.NormalCurrencyCode, out var cacheEntry) && !HasExpired(cacheEntry))
                {
                    rate = cacheEntry.Rate;
                    return true;
                }
            }

            rate = default;
            return false;
        }

        private ConcurrencyGate AcquireGate(string currencyCode)
        {
            string normal = Normalise.CurrencyCode(currencyCode);

            lock (_mainGate)
            {
                if (!_concurrencyGates.TryGetValue(normal, out var gate))
                {
                    gate = new(normal);
                    _concurrencyGates[normal] = gate;
                }

                gate.RefCount++;
                return gate;
            }
        }

        private void StoreCachedRate(ConcurrencyGate gate, decimal rate)
        {
            var cacheEntry = new CacheEntry(rate, _timeProvider.GetUtcNow());
            lock (_mainGate)
                _cache[gate.NormalCurrencyCode] = cacheEntry;
        }

        private void ReleaseGate(ConcurrencyGate gate)
        {
            lock (_mainGate)
            {
                gate.RefCount--;
                if (gate.RefCount == 0)
                {
                    if (_concurrencyGates.TryGetValue(gate.NormalCurrencyCode, out var otherGate))
                    {
                        if (ReferenceEquals(gate, otherGate))
                        {
                            _concurrencyGates.Remove(gate.NormalCurrencyCode);
                            gate.Dispose();
                        }
                    }
                }
            }
        }

        private bool HasExpired(CacheEntry cacheEntry)
        {
            return (_timeProvider.GetUtcNow() - cacheEntry.Timestamp).TotalMilliseconds > _ttlMs;
        }

        public void Dispose()
        {
            lock (_mainGate)
            {
                if (!_disposed)
                {
                    _disposed = true;

                    _mainSemaphore.Dispose();
                    foreach (var gate in _concurrencyGates.Values)
                        gate.Dispose();
                    _concurrencyGates.Clear();
                    _currencyCodes.Clear();
                }
            }
        }

        private void ThrowIfDisposed() // Reject calls made after this service has been disposed.
        {
            lock (_mainGate)
            {
                ObjectDisposedException.ThrowIf(_disposed, this);
            }
        }
    }
}

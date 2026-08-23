using static TrustedAbstractions.Abstractions;

namespace TrustedCachingRateApi
{
    public class CachingRateApi : IRateApi
    {
        private const int MaxTtlMs = 10;
        private sealed record CacheEntry(decimal Rate, DateTimeOffset Timestamp);

        private sealed class ConcurrencyGate(string normalCurrencyCode)
        {
            public int RefCount { get; set; }
            public Lock Gate { get; } = new();
            public string NormalCurrencyCode { get; } = normalCurrencyCode;
        }

        private readonly int _ttlMs;
        private readonly IRateApi _realApi;
        private readonly Lock _mainGate = new();
        private readonly TimeProvider _timeProvider;
        private readonly Dictionary<string, CacheEntry> _cache = [];
        private readonly Dictionary<string, ConcurrencyGate> _concurrencyGates = [];

        public CachingRateApi(IRateApi realApi, int ttlMs, TimeProvider timeProvider)
        {
            ArgumentNullException.ThrowIfNull(realApi);
            ArgumentNullException.ThrowIfNull(timeProvider);
            if (ttlMs < MaxTtlMs) throw new ArgumentException($"Invalid ttlMs: {ttlMs}ms", nameof(ttlMs));

            _ttlMs = ttlMs;
            _realApi = realApi;
            _timeProvider = timeProvider;
        }

        public decimal GetUsdRate(string currencyCode)
        {
            var gate = AcquireGate(currencyCode);

            try
            {
                lock (gate.Gate)
                {
                    if (TryGetCachedRate(gate, out var rate))
                        return rate;

                    rate = _realApi.GetUsdRate(gate.NormalCurrencyCode);
                    StoreCachedRate(gate, rate);
                    return rate;
                }
            }
            finally
            {
                ReleaseGate(gate);
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
            string normal = NormaliseCurrencyCode(currencyCode);

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
                            _concurrencyGates.Remove(gate.NormalCurrencyCode);
                    }
                }
            }
        }

        private bool HasExpired(CacheEntry cacheEntry)
        {
            return (_timeProvider.GetUtcNow() - cacheEntry.Timestamp).TotalMilliseconds > _ttlMs;
        }

        private static string NormaliseCurrencyCode(string currencyCode)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(currencyCode);

            return currencyCode.ToUpperInvariant().Trim() switch
            {
                "USD" => "USD",
                "GBP" => "GBP",
                "JPY" => "JPY",
                "CAD" => "CAD",
                "EUR" => "EUR",
                _ => throw new ArgumentException($"Invalid CurrencyCode: {currencyCode}", nameof(currencyCode))
            };
        }
    }
}

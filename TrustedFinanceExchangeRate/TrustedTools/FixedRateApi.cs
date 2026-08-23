using TrustedAbstractions;

namespace TrustedTools
{
    public sealed class FixedRateApi : IRateApi
    {
        private static readonly IReadOnlyDictionary<string, decimal> Rates =
            new Dictionary<string, decimal>(StringComparer.Ordinal)
            {
                [Normalise.CurrencyCode("USD")] = 1.0000m,
                [Normalise.CurrencyCode("EUR")] = 1.0850m,
                [Normalise.CurrencyCode("GBP")] = 1.2650m,
                [Normalise.CurrencyCode("JPY")] = 0.0067m,
                [Normalise.CurrencyCode("CAD")] = 0.7400m
            };

        public async Task<decimal> GetUsdRate(string currencyCode)
        {
            var normalizedCode = Normalise.CurrencyCode(currencyCode);

            await Task.Delay(3000);

            if (!Rates.TryGetValue(normalizedCode, out var rate))
                throw new ArgumentException(
                    $"Unsupported currency code: {currencyCode}",
                    nameof(currencyCode));

            return rate;
        }

        public async Task<IReadOnlyList<string>> GetSupportedCurrencyCodes()
        {
            await Task.Delay(100);
            return [.. Rates.Select(rate => rate.Key)];
        }
    }
}

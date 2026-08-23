namespace TrustedTools
{
    public static class Normalise
    {
        public static string CurrencyCode(string currencyCode)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(currencyCode);
            return currencyCode.ToUpperInvariant().Trim();
        }
    }
}

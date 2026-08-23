namespace TrustedAbstractions
{
    public class Abstractions
    {
        /// <summary>
        /// Remote FX rate provider.
        /// </summary>
        /// <remarks>
        /// Every call is a network round trip against a metered third-party API.
        /// Budget roughly 50ms per call and treat calls as something you pay for.
        /// This is the dependency the caching work exists to protect.
        /// </remarks>
        public interface IRateApi
        {
            /// <summary>
            /// Returns how many USD one unit of <paramref name="currencyCode"/> is worth.
            /// </summary>
            /// <exception cref="ArgumentException">The currency is not supported.</exception>
            decimal GetUsdRate(string currencyCode);
        }
    }
}

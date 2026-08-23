using System.Net;
using System.Net.Http.Json;
using TrustedTools;
using TrustedAbstractions;

namespace TrustedTests.WebApiTests
{
    [Collection(TestGroupNames.WebApi)]
    public class RatesControllerTests(TrustedWebApiFactory factory) : IClassFixture<TrustedWebApiFactory>, IAsyncLifetime
    {
        public Task InitializeAsync() => Task.CompletedTask;
        public Task DisposeAsync() => Task.CompletedTask;

        private readonly HttpClient _client = factory.CreateClient();

        private async Task<RateRecord?> GetCurrencyCode(string currencyCode)
        {
            var response = await _client.GetAsync($"/rates/{currencyCode}");

            if (response.IsSuccessStatusCode)
                return await response.Content.ReadFromJsonAsync<RateRecord>();

            if (response.StatusCode == HttpStatusCode.NotFound)
                return null;

            throw new InvalidOperationException($"Failed to get currency code: {currencyCode}");
        }

        private async Task<List<string>> GetSupportedCurrencyCodes()
        {
            var response = await _client.GetAsync("/rates");

            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<List<string>>()
                ?? throw new InvalidOperationException(
                    "The supported currency response was empty.");
        }

        [Theory]
        [InlineData("USD")] 
        [InlineData("EUR")]
        [InlineData("GBP")]
        public async Task CanGetCurrencyCode(string currencyCode)
        {
            var rateRecord = await GetCurrencyCode(currencyCode);
            Assert.NotNull(rateRecord);
            Assert.True(rateRecord.Rate > 0m);
            Assert.Equal(Normalise.CurrencyCode(currencyCode), rateRecord.CurrencyCode);
        }

        [Fact]
        public async Task CanGetSupportedCurrencyCodes()
        {
            var currencyCodes = await GetSupportedCurrencyCodes();

            Assert.NotEmpty(currencyCodes);
            Assert.Contains("USD", currencyCodes);
            Assert.Contains("EUR", currencyCodes);
            Assert.Contains("GBP", currencyCodes);
            Assert.Contains("JPY", currencyCodes);
            Assert.Contains("CAD", currencyCodes);
        }

        [Fact]
        public async Task ReturnsAllSupportedCurrencyCodes()
        {
            var currencyCodes = await GetSupportedCurrencyCodes();

            Assert.Equal(
                ["USD", "EUR", "GBP", "JPY", "CAD"],
                currencyCodes);
        }

        [Theory]
        [InlineData("usd", "USD")]
        [InlineData(" eur ", "EUR")]
        [InlineData("GbP", "GBP")]
        public async Task NormalizesCurrencyCode(
            string requestedCode,
            string expectedCode)
        {
            var rateRecord = await GetCurrencyCode(requestedCode);

            Assert.NotNull(rateRecord);
            Assert.Equal(expectedCode, rateRecord.CurrencyCode);
        }

        [Theory]
        [InlineData("AUD")]
        [InlineData("XYZ")]
        public async Task UnsupportedCurrencyCodeReturnsNotFound(
            string currencyCode)
        {
            var response = await _client.GetAsync($"/rates/{currencyCode}");

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
    }
}

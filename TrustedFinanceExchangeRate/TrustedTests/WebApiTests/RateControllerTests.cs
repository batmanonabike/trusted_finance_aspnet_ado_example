using System.Net;
using System.Net.Http.Json;
using TrustedTools;
using TrustedAbstractions;

namespace TrustedTests.WebApiTests
{
    [Collection(TestGroupNames.WebApi)]
    public class RateControllerTests(TrustedWebApiFactory factory) : IClassFixture<TrustedWebApiFactory>, IAsyncLifetime
    {
        public Task InitializeAsync() => Task.CompletedTask;
        public Task DisposeAsync() => Task.CompletedTask;

        private readonly HttpClient _client = factory.CreateClient();

        private async Task<RateRecord?> GetCurrencyCode(string currencyCode)
        {
            var response = await _client.GetAsync($"/rate/{currencyCode}");

            if (response.IsSuccessStatusCode)
                return await response.Content.ReadFromJsonAsync<RateRecord>();

            if (response.StatusCode == HttpStatusCode.NotFound)
                return null;

            throw new InvalidOperationException($"Failed to get currency code: {currencyCode}");
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
    }
}

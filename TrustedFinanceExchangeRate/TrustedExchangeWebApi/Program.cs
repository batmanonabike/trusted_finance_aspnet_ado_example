namespace TrustedExchangeWebApi
{
    // Not using top level statements for readability in the tests.
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddCachingRateApi();
            builder.Services.AddControllers();
            builder.Services.AddOpenApi();

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
                app.MapOpenApi();

            if (!app.Environment.IsEnvironment("Testing"))
                app.UseHttpsRedirection();

            app.UseAuthorization();
            app.MapControllers();

            app.Lifetime.ApplicationStarted.Register(() => LogStartupUrls(app));

            app.Run();
        }

        private static void LogStartupUrls(WebApplication app)
        {
            var baseUrl = app.Urls.FirstOrDefault() ?? "http://localhost:5165";
            app.Logger.LogInformation(
                "Try it:\n" +
                "  1) {CodesUrl} <- returns all supported currency codes\n" +
                "  2) {EurUrl} <- returns the exchange rate for EUR\n" +
                "  3) {GbpUrl} <- returns the exchange rate for GBP\n" +
                "(rates are cached for a while, so repeat hits will be faster)",
                $"{baseUrl}/rates/currencycodes", $"{baseUrl}/rates/EUR", $"{baseUrl}/rates/GBP");
        }
    }
}
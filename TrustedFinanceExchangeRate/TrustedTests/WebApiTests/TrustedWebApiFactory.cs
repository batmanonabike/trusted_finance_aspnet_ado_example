using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;

namespace TrustedTests.WebApiTests
{
    public sealed class TrustedWebApiFactory : WebApplicationFactory<TrustedExchangeWebApi.Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("Testing");
        }
    }
}

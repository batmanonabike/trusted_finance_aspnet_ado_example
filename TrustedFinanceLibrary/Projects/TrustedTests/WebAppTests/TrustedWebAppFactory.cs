using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;

namespace TrustedTests.WebAppTests
{
    // This handy: WebApplicationFactory that I just found runs the web app in memory.
    // https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.testing.webapplicationfactory-1?view=aspnetcore-10.0&devlangs=csharp&f1url=%3FappId%3DDev18IDEF1%26l%3DEN-US%26k%3Dk%28Microsoft.AspNetCore.Mvc.Testing.WebApplicationFactory%601%29%3Bk%28DevLang-csharp%29%26rd%3Dtrue
    public sealed class TrustedWebAppFactory : WebApplicationFactory<TrustedWebApp.Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureAppConfiguration((_, config) =>
            {
                // Lets just use JSON. 
                config.AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["TrustedLibrary:UseSqlDatabase"] = "false",
                });
            });
        }
    }
}

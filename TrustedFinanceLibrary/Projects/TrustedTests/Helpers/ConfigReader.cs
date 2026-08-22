using Microsoft.Extensions.Configuration;

namespace TrustedTests.Helpers
{
    public sealed class ConfigReader
    {
        public IConfiguration Configuration { get; }
        public string LibraryConnectionString => GetLibraryConnectionString();

        public ConfigReader()
        {
            Configuration = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false)
                .Build();
        }

        private string GetLibraryConnectionString()
        {
            return Configuration.GetConnectionString("Library")
                ?? throw new InvalidOperationException(
                "ConnectionStrings:Library is missing.");
        }
    }
}

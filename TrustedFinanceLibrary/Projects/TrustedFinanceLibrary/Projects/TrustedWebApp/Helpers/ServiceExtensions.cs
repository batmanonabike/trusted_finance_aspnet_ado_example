using Microsoft.Extensions.Options;
using TrustedAbstractions;
using TrustedWebApp.Services;

namespace TrustedWebApp.Helpers
{
    internal sealed class LibraryOptions
    {
        public bool UseSqlDatabase { get; set; }
    }

    public static class ServiceCollectionExtensions
    {
        public static void AddTrustedLibrary(this IServiceCollection services)
        {
            services.AddScoped<ILibraryService>(GetLibraryService);
            services.AddOptions<LibraryOptions>().BindConfiguration("TrustedLibrary");

            services.AddScoped<TrustedSqlDatabase.Library, TrustedSqlDatabase.Library>();
            services.AddSingleton<TrustedSqlDatabase.LibrarySettings>(GetSqlLibrarySettings);

            services.AddScoped<TrustedJsonDatabase.Library, TrustedJsonDatabase.Library>();
            services.AddSingleton<TrustedJsonDatabase.LibrarySettings>();
        }

        private static ILibraryService GetLibraryService(IServiceProvider serviceProvider)
        {
            var options = serviceProvider.GetRequiredService<IOptions<LibraryOptions>>().Value;
            var library = GetLibrary(serviceProvider, options.UseSqlDatabase);
            return new LibraryService(library);
        }

        private static ILibrary GetLibrary(IServiceProvider serviceProvider, bool useSqlDatabase)
        {
            return useSqlDatabase ?
                serviceProvider.GetRequiredService<TrustedSqlDatabase.Library>() :
                serviceProvider.GetRequiredService<TrustedJsonDatabase.Library>();
        }

        private static TrustedSqlDatabase.LibrarySettings GetSqlLibrarySettings(IServiceProvider serviceProvider)
        {
            var config = serviceProvider.GetRequiredService<IConfiguration>();
            var connectionString = config.GetConnectionString("TrustedLibrary")
                ?? throw new InvalidOperationException("ConnectionStrings:TrustedLibrary is missing.");
            return new TrustedSqlDatabase.LibrarySettings { ConnectionString = connectionString };
        }
    }
}

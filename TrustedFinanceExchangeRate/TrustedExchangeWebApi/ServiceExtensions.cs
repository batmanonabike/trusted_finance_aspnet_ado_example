using TrustedAbstractions;
using TrustedCachingRateApi;
using TrustedTools;

namespace TrustedExchangeWebApi;

public static class ServiceExtensions
{
    public static IServiceCollection AddCachingRateApi(
        this IServiceCollection services)
    {
        services.AddSingleton<FixedRateApi>();
        services.AddSingleton(TimeProvider.System);
        services.AddSingleton<IRateApi>(serviceProvider =>
        {
            var configuration = serviceProvider.GetRequiredService<IConfiguration>();
            var configuredTtl = configuration["CachingRateApi:TtlMs"];

            if (!int.TryParse(configuredTtl, out var ttlMs))
            {
                throw new InvalidOperationException(
                    "Configuration value 'CachingRateApi:TtlMs' must be an integer.");
            }

            return new CachingRateApi(
                ttlMs,
                serviceProvider.GetRequiredService<FixedRateApi>(),
                serviceProvider.GetRequiredService<TimeProvider>());
        });

        return services;
    }
}

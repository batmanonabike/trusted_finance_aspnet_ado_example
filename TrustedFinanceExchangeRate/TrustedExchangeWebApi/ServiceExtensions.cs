using TrustedAbstractions;
using TrustedCachingRateApi;
using TrustedTools;
using Microsoft.Extensions.Options;

namespace TrustedExchangeWebApi;

public sealed class CachingRateApiOptions
{
    public int TtlMs { get; set; }
    public int MinTtlMs { get; set; }
}

public static class ServiceExtensions
{
    public static IServiceCollection AddCachingRateApi(
        this IServiceCollection services)
    {
        services.AddCachingRateApiOptions();
        services.AddSingleton<FixedRateApi>();
        services.AddSingleton(TimeProvider.System);
        services.AddSingleton<IRateApi>(CreateCachingRateApi);

        return services;
    }

    private static IServiceCollection AddCachingRateApiOptions(
        this IServiceCollection services)
    {
        services.AddOptions<CachingRateApiOptions>()
            .BindConfiguration("CachingRateApi")
            .Validate(
                options => options.MinTtlMs > 0,
                "CachingRateApi:MinTtlMs must be greater than zero.")
            .Validate(
                options => options.TtlMs >= options.MinTtlMs,
                "CachingRateApi:TtlMs must be greater than or equal to CachingRateApi:MinTtlMs.")
            .ValidateOnStart();

        return services;
    }

    private static IRateApi CreateCachingRateApi(
        IServiceProvider serviceProvider)
    {
        var options = serviceProvider
            .GetRequiredService<IOptions<CachingRateApiOptions>>()
            .Value;

        return new CachingRateApi(
            options.TtlMs,
            options.MinTtlMs,
            serviceProvider.GetRequiredService<FixedRateApi>(),
            serviceProvider.GetRequiredService<TimeProvider>());
    }
}

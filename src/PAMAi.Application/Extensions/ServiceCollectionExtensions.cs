using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PAMAi.Application.Services;
using PAMAi.Application.Services.Interfaces;
using PAMAi.Domain.Options;

namespace PAMAi.Application.Extensions;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Add application layer.
    /// </summary>
    /// <param name="services">
    /// Services container.
    /// </param>
    /// <returns>
    /// <see cref="IServiceCollection"/>.
    /// </returns>
    /// <param name="configuration"></param>
    public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddServices();
        services.ConfigureOptions(configuration);
        services.AddValidatorsFromAssemblyContaining<Result>();

        return services;
    }

    private static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddScoped<IAssetService, AssetService>();
        services.AddScoped<ICountryService, CountryService>();

        return services;
    }

    private static IServiceCollection ConfigureOptions(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<JwtOptions>(configuration.GetSection(JwtOptions.ConfigurationKey));

        return services;
    }
}

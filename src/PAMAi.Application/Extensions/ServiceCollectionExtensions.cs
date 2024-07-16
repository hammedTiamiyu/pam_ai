using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

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
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddServices();
        services.AddValidatorsFromAssemblyContaining<Result>();

        return services;
    }

    private static IServiceCollection AddServices(this IServiceCollection services)
    {
        // Add services here.

        return services;
    }
}

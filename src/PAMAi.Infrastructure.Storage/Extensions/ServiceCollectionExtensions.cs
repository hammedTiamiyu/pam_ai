using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PAMAi.Application.Exceptions;
using PAMAi.Application.Storage;
using PAMAi.Infrastructure.Storage.Contexts;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;

namespace PAMAi.Infrastructure.Storage.Extensions;
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers the DbContext, unit of work and services required for including
    /// a full identity layer to the application.
    /// </summary>
    /// <param name="services">
    /// Services container.
    /// </param>
    /// <param name="configuration">
    /// Application configuration.
    /// </param>
    /// <returns>
    /// The updated services container.
    /// </returns>
    /// <exception cref="ConfigurationException"></exception>
    public static IServiceCollection AddStorageInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Default");

        if (string.IsNullOrWhiteSpace(connectionString))
            throw new ConfigurationException("Connection string is null or whitespace.", "ConnectionStrings:Default");

        services.AddDbContext(connectionString);
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        return services;
    }

    private static IServiceCollection AddDbContext(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
        {
#if DEBUG
            // Log more detailed errors if debugging.
            options.EnableDetailedErrors();
            options.EnableSensitiveDataLogging();
#endif
            ServerVersion serverVersion = ServerVersion.AutoDetect(connectionString);
            options.UseMySql(connectionString, serverVersion, options =>
            {
                options.EnableRetryOnFailure();
                options.EnableStringComparisonTranslations();
                options.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName);
                options.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
                options.SchemaBehavior(MySqlSchemaBehavior.Ignore);
            });
        });

        return services;
    }

}

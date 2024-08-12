using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PAMAi.Application.Exceptions;
using PAMAi.Application.Services.Interfaces;
using PAMAi.Infrastructure.Identity.Models;
using PAMAi.Infrastructure.Identity.Seed;
using PAMAi.Infrastructure.Identity.Services;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;

namespace PAMAi.Infrastructure.Identity.Extensions;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers the DbContext, ASP.NET Core identity and services required for including
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
    public static IServiceCollection AddIdentityInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Default");

        if (string.IsNullOrWhiteSpace(connectionString))
            throw new ConfigurationException("Connection string is null or whitespace.", "ConnectionStrings:Default");

        services.AddIdentityContext(connectionString);
        services.AddApplicationAuthentication(configuration);
        services.AddApplicationIdentity(configuration);
        services.AddServices();
        services.AddDistributedMemoryCache();

        return services;
    }

    private static IServiceCollection AddIdentityContext(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<IdentityContext>(options =>
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
                options.MigrationsAssembly(typeof(IdentityContext).Assembly.FullName);
                options.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
                options.SchemaBehavior(MySqlSchemaBehavior.Ignore);
            });
        });

        return services;
    }

    private static IServiceCollection AddApplicationAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAuthentication().AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
        {
            options.SaveToken = true;
            options.RequireHttpsMetadata = false;

            string issuer = configuration["Authentication:Schemes:Bearer:Issuer"] ?? "";
            string audience = configuration["Authentication:Schemes:Bearer:Audience"] ?? "";
            options.TokenValidationParameters = Constants.Jwt.GetApplicationTokenValidationParameters(issuer, audience);
        });
        services.AddAuthorization();

        return services;
    }

    private static IServiceCollection AddApplicationIdentity(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddIdentity<User, IdentityRole>(options =>
        {
            options.Password.RequiredLength = Constants.PasswordRequirements.MinLength;
            options.Password.RequireNonAlphanumeric = Constants.PasswordRequirements.RequireNonAlphanumeric;
            options.Password.RequireUppercase = Constants.PasswordRequirements.RequireUppercase;
            options.Password.RequireLowercase = Constants.PasswordRequirements.RequireLowercase;
            options.Password.RequireDigit = Constants.PasswordRequirements.RequireDigit;

            options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(configuration.GetValue<int>("Authentication:DefaultLockoutTimeSpan"));
            options.Lockout.MaxFailedAccessAttempts = configuration.GetValue<int>("Authentication:MaxFailedAccessAttempts");
            options.Lockout.AllowedForNewUsers = true;

            options.User.RequireUniqueEmail = true;
        }).AddEntityFrameworkStores<IdentityContext>().AddDefaultTokenProviders();

        services.Configure<DataProtectionTokenProviderOptions>(config => config.TokenLifespan = TimeSpan.FromMinutes(2));

        return services;
    }

    private static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddScoped<Seeder>();
        services.AddScoped<IAccountService, AccountService>();
        services.AddScoped<IAuthenticationService, AuthenticationService>();
        services.AddScoped<ICurrentUser, CurrentUser>();
        services.AddScoped<ITokenService, TokenService>();

        return services;
    }
}


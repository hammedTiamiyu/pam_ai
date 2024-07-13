using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using PAMAi.Application.Exceptions;
using PAMAi.Infrastructure.Identity.Models;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;

namespace PAMAi.Infrastructure.Identity.Extensions;

public static class ServiceExtensions
{
    public static IServiceCollection AddIdentityInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Identity");

        if (string.IsNullOrWhiteSpace(connectionString))
            throw new ConfigurationException("Connection string is null or whitespace.", "ConnectionStrings:Identity");

        services.AddIdentityContext(connectionString);
        services.AddApplicationAuthentication(configuration);
        services.AddApplicationIdentity(configuration);

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
        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(options =>
        {
            options.SaveToken = false;
            options.RequireHttpsMetadata = false;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ClockSkew = TimeSpan.Zero,
                ValidIssuer = configuration["Authentication:Schemes:Bearer:Issuer"],
                ValidAudience = configuration["Authentication:Schemes:Bearer:Audience"],
                ValidAlgorithms = Constants.Jwt.Algorithms,

                ValidateIssuerSigningKey = true,
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                LogValidationExceptions = true,
                RequireExpirationTime = true,

                IssuerSigningKey = Constants.Jwt.SecurityKey,
            };
        });

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

        return services;
    }
}


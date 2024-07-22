using Microsoft.EntityFrameworkCore.Query.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PAM_Ai.ExternalServices.Services.FcmNotifications;
using PAMAi.Application.Exceptions;
using PAMAi.Application.Services.Interfaces;
using PAMAi.Domain.Enums;
using PAMAi.Domain.Options;
using PAMAi.Infrastructure.ExternalServices.Services;
using PAMAi.Infrastructure.ExternalServices.Services.SMS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PAMAi.Infrastructure.ExternalServices.Extensions;
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// contains thirdparty impmenetations e.g Termii for SMS notifs
    /// </summary>
    /// <param name="services">
    /// Conatiner
    /// </param>
    /// <param name="configuration">
    /// Contains ext. services configs</param>
    /// <returns></returns>
    /// <exception cref="ConfigurationException"></exception>
    /// <returns>
    /// The updated services container.
    /// </returns>
    /// <exception cref="ConfigurationException"></exception>
    public static IServiceCollection AddExternalServicesInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.ConfigureOptions(configuration);
        services.AddSmsSettings(configuration);
        services.AddFcmSettings(configuration);
        services.AddServices();
        return services;

    }

    private static IServiceCollection AddSmsSettings(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<TermiiOptions>(configuration.GetSection("TermiiSettings"));
        services.AddSingleton<SmsRepository>(provider =>
        {
            var logger = provider.GetRequiredService<ILogger<SmsRepository>>();
            return new SmsRepository(configuration, logger);
        });
        return services;
    }

    private static IServiceCollection AddFcmSettings(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<FcmService>(provider =>
        {
            var logger = provider.GetRequiredService<ILogger<FcmService>>();
            return new FcmService(configuration, logger);
        });
   
        return services;
    }

    private static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddScoped<ISmsRepository, SmsRepository>();
        services.AddScoped<INotificationService, NotificationService>();
        return services;
    }
    private static IServiceCollection ConfigureOptions(this IServiceCollection services, IConfiguration configuration)
    {
        
        return services;
    }

}

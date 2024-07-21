using Microsoft.EntityFrameworkCore.Query.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PAM_Ai.ExternalServices.Services.SMS;
using PAMAi.Application.Exceptions;
using PAMAi.Application.Services.Interfaces;
using PAMAi.Domain.Enums;
using PAMAi.Domain.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PAM_Ai.ExternalServices.Extensions;
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
        services.AddServices();
        return services;

    }

    private static IServiceCollection AddSmsSettings(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<TermiiOptions>(configuration.GetSection("TermiiSettings"));
        return services;
    }

    private static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddScoped<ISmsRepository, SmsRepository>();
        return services;
    }
    private static IServiceCollection ConfigureOptions(this IServiceCollection services, IConfiguration configuration)
    {
        
        return services;
    }

}

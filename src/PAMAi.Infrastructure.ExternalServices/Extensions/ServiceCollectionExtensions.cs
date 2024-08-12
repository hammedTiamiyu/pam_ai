using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PAMAi.Application.Exceptions;
using PAMAi.Application.Services.Interfaces;
using PAMAi.Domain.Options;
using PAMAi.Infrastructure.ExternalServices.Services.Email;
using PAMAi.Infrastructure.ExternalServices.Services.PushNotification;
using PAMAi.Infrastructure.ExternalServices.Services.SMS;

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
        services.AddServices();
        return services;

    }

    private static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddScoped<IEmailService, EmailService>();
        services.AddSingleton<IPushNotificationService, PushNotificationService>();
        services.AddSingleton<ISmsService, SmsService>();

        return services;
    }
    private static IServiceCollection ConfigureOptions(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<TermiiOptions>(configuration.GetSection("TermiiSettings"));
        services.Configure<EmailOptions>(configuration.GetSection(EmailOptions.ConfigurationKey));

        return services;
    }
}

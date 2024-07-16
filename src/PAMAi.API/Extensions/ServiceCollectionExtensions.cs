namespace PAMAi.API.Extensions;

internal static class ServiceCollectionExtensions
{
    internal static ApiVersion s_defaultApiVersion = new(1.0);

    /// <summary>
    /// Configure API versioning for the application and register it in the service collection.
    /// </summary>
    /// <param name="services">Service collection.</param>
    /// <returns></returns>
    internal static IServiceCollection AddApiVersioningInternal(this IServiceCollection services)
    {
        services.AddApiVersioning(options =>
        {
            options.DefaultApiVersion = s_defaultApiVersion;
            options.ReportApiVersions = true;
            options.AssumeDefaultVersionWhenUnspecified = true;
            options.ApiVersionReader = ApiVersionReader.Combine(new UrlSegmentApiVersionReader());
        }).AddApiExplorer(options =>
        {
            options.GroupNameFormat = "'v'VVV";
            options.SubstituteApiVersionInUrl = true;
        });

        return services;
    }
}

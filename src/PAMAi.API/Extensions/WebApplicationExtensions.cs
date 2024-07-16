using System.Diagnostics;
using Asp.Versioning.ApiExplorer;
using Microsoft.EntityFrameworkCore;
using PAMAi.Infrastructure.Identity;
using PAMAi.Infrastructure.Storage.Contexts;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace PAMAi.API.Extensions;

public static class WebApplicationExtensions
{
    /// <summary>
    /// Apply migration changes to the database. Will create the database if it doesn't exist.
    /// </summary>
    /// <param name="app"></param>
    /// <returns></returns>
    public static async Task<WebApplication> MigrateDatabasesAsync(this WebApplication app)
    {
        long identityMigrationTime;
        long applicationMigrationTime;

        await using var scope = app.Services.CreateAsyncScope();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<ApplicationDbContext>>();
        using var identityContext = scope.ServiceProvider.GetRequiredService<IdentityContext>();
        using var applicationDbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        logger.LogDebug("Migrating databases");
        var watch = Stopwatch.StartNew();
        await identityContext.Database.MigrateAsync();
        watch.Stop();
        identityMigrationTime = watch.ElapsedMilliseconds;
        logger.LogDebug("{Context} migration finished in {Time} ms",
            identityContext.GetType().Name,
            identityMigrationTime);

        watch.Restart();
        await applicationDbContext.Database.MigrateAsync();
        watch.Stop();
        applicationMigrationTime = watch.ElapsedMilliseconds;
        logger.LogDebug("{Context} migration finished in {Time} ms",
            applicationDbContext.GetType().Name,
            applicationMigrationTime);

        logger.LogDebug("All database migrations finished in {Time} ms", identityMigrationTime + applicationMigrationTime);

        return app;
    }

    /// <summary>
    /// Use Swagger documentation.
    /// </summary>
    /// <param name="app">Application builder.</param>
    /// <returns></returns>
    public static IApplicationBuilder UseSwaggerInternal(this WebApplication app)
    {
        var apiVersionDescriptionProvider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();
        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            foreach (var description in apiVersionDescriptionProvider.ApiVersionDescriptions)
            {
                options.SwaggerEndpoint($"../swagger/{description.GroupName}/swagger.json", description.GroupName.ToUpperInvariant());
#if DEBUG
                options.DisplayRequestDuration();
#endif
                options.DocExpansion(DocExpansion.None);
                options.EnableDeepLinking();
                options.EnableFilter();
                options.EnablePersistAuthorization();
                options.EnableValidator();
                options.ShowCommonExtensions();
                options.ShowExtensions();
            }
        });

        return app;
    }
}

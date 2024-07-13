using Microsoft.EntityFrameworkCore;
using PAMAi.Infrastructure.Identity;

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
        await using var scope = app.Services.CreateAsyncScope();
        using var identityContext = scope.ServiceProvider.GetRequiredService<IdentityContext>();

        await identityContext.Database.MigrateAsync();

        return app;
    }
}

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using PAMAi.Infrastructure.Identity.Seed;

namespace PAMAi.Infrastructure.Identity.Extensions;
public static class WebApplicationExtensions
{
    /// <summary>
    /// Seed all data required for the identity layer.
    /// </summary>
    /// <param name="app">
    /// Web application.
    /// </param>
    /// <returns>
    /// Web application.
    /// </returns>
    public static async Task<WebApplication> SeedIdentityDatabaseAsync(this WebApplication app)
    {
        await using AsyncServiceScope scope = app.Services.CreateAsyncScope();
        Seeder seeder = scope.ServiceProvider.GetRequiredService<Seeder>();

        await seeder.CreateApplicationRolesAsync();
        await seeder.CreateDefaultSuperAdminAsync();

        return app;
    }
}

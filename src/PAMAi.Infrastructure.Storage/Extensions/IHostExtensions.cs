using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PAMAi.Infrastructure.Storage.Seed;

namespace PAMAi.Infrastructure.Storage.Extensions;
public static class IHostExtensions
{
    public static async Task<IHost> SeedStorageDatabaseAsync(this IHost app)
    {
        await using AsyncServiceScope scope = app.Services.CreateAsyncScope();
        Seeder seeder = scope.ServiceProvider.GetRequiredService<Seeder>();

        await seeder.SeedCountriesAsync();
        await seeder.SeedDefaultTermsOfServiceAsync();

        return app;
    }
}

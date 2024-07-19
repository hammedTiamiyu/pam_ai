using Microsoft.Extensions.Logging;

namespace PAMAi.Infrastructure.Storage.Seed;
internal sealed partial class Seeder
{
    internal async Task SeedCountriesAsync()
    {
        _logger.LogInformation("Seeding countries and states");
        // Use offline file to speed up application start up.
        await _countryService.UpdateCountriesAndStatesAsync(false);
    }
}

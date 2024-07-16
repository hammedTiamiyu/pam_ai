using System.Diagnostics;
using System.Net.Http.Json;
using Mapster;
using Microsoft.Extensions.Logging;
using PAMAi.Infrastructure.Storage.Seed.Models;

namespace PAMAi.Infrastructure.Storage.Seed;
internal sealed partial class Seeder
{
    private int _countriesAdded = 0;
    private long _statesAdded = 0;

    internal async Task SeedCountriesAsync()
    {
        _logger.LogInformation("Seeding countries and states");

        using HttpClient client = _httpClientFactory.CreateClient();
        string apiEndpoint = "https://countriesnow.space/api/v0.1/countries/states";

        try
        {
            var watch = Stopwatch.StartNew();
            var response = await client.GetFromJsonAsync<CountryStateApiResponse>(apiEndpoint);
            watch.Stop();

            if (response is null)
            {
                _logger.LogWarning("API response was null or empty");
                return;
            }

            if (response.HasError)
            {
                _logger.LogWarning("API response indicates an error. Response: {@Response}", response);
                return;
            }

            _logger.LogDebug("GET {URI} returned {Count} countries in {Time} ms",
                apiEndpoint,
                response.Countries.Count,
                watch.ElapsedMilliseconds);

            watch.Restart();
            foreach (Models.Country apiCountry in response.Countries)
            {
                Domain.Entities.Country country = apiCountry.Adapt<Domain.Entities.Country>();

                await AddCountryAsync(country);
            }
            await _unitOfWork.CompleteAsync();
            watch.Stop();

            _logger.LogDebug("{Count} countries and {Count} states added in {Time} ms",
                _countriesAdded,
                _statesAdded,
                watch.ElapsedMilliseconds);
            _logger.LogInformation("Countries and states seeded");
        }
        catch (Exception exception)
        {
            _logger.LogError(
                exception,
                "An error occurred while seeding countries and states. Message: {Message}",
                exception.Message);
        }
    }

    private async Task AddCountryAsync(Domain.Entities.Country country)
    {
        Domain.Entities.Country? dbCountry = await _unitOfWork.Countries.FindByNameAsync(country.Name);
        if (dbCountry is null)
        {
            _logger.LogTrace("Country '{Country}' does not exist in database. Now inserting it", country.Name);

            await _unitOfWork.Countries.AddAsync(country);

            _countriesAdded++;
            _statesAdded += country.States.Count;

            return;
        }

        _logger.LogTrace("Country '{Country}' already exists. Comparing country's states", country.Name);

        foreach (Domain.Entities.State state in country.States)
        {
            state.CountryId = dbCountry.Id;
        }

        // Find states that are not in the database and insert them.
        IEnumerable<Domain.Entities.State> statesNotInDb = country.States.Except(dbCountry.States);
        if (statesNotInDb.Any())
        {
            _logger.LogDebug("{Count} states of country '{Country}' are not in the database. Inserting them",
                statesNotInDb.LongCount(),
                country.Name);


            await _unitOfWork.States.AddRangeAsync(statesNotInDb);
            _statesAdded += statesNotInDb.LongCount();

            return;
        }

        _logger.LogTrace("All states of country '{Country}' are up to date", country.Name);
    }
}

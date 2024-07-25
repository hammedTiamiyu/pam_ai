using System.Diagnostics;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using PAMAi.Application.Dto.Country;
using PAMAi.Application.Errors;
using PAMAi.Application.Services.Interfaces;
using PAMAi.Application.Services.Models.Internal;
using PAMAi.Application.Storage;
using PAMAi.Domain.Entities;

namespace PAMAi.Application.Services;

internal class CountryService: ICountryService
{
    private static readonly string s_fileName = "countries.json";
    private static readonly string s_filePath = Path.Combine(AppContext.BaseDirectory, s_fileName);
    private static readonly JsonSerializerOptions s_jsonSerializerOptions = new JsonSerializerOptions
    {
        AllowTrailingCommas = true,
        WriteIndented = true,
    };

    private int _countriesAdded;
    private long _statesAdded;
    private readonly ILogger<CountryService> _logger;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IUnitOfWork _unitOfWork;

    public CountryService(ILogger<CountryService> logger, IHttpClientFactory httpClientFactory, IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _httpClientFactory = httpClientFactory;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<List<ReadCountryResponse>>> GetCountriesAsync(CancellationToken cancellationToken = default)
    {
        IEnumerable<Country> countries = await _unitOfWork.Countries.GetAsync(cancellationToken);
        List<ReadCountryResponse> response = countries.Adapt<List<ReadCountryResponse>>();

        return Result<List<ReadCountryResponse>>.Success(response);
    }

    public async Task<Result<List<ReadCountryStateResponse>>> GetCountryStatesAsync(int countryId, CancellationToken cancellationToken = default)
    {
        Country? country = await _unitOfWork.Countries.FindAsync(countryId, cancellationToken);
        if (country is null)
        {
            _logger.LogError("Country {Id} does not exist", countryId);
            return Result<List<ReadCountryStateResponse>>.Failure(DefaultErrors.NotFound with
            {
                Description = $"Country {countryId} does not exist.",
            });
        }

        List<ReadCountryStateResponse> response = country.States.Adapt<List<ReadCountryStateResponse>>();
        _logger.LogDebug("Number of states of country '{Name}: {Count}", country.Name, response.Count);

        return Result<List<ReadCountryStateResponse>>.Success(response);
    }

    public async Task<Result> UpdateCountriesAndStatesAsync(bool fromInternet = true, bool fallbackOnError = true, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Updating countries and states. Use internet: {UseInternet}. Use fallback: {UseFallback}",
            fromInternet,
            fallbackOnError);

        try
        {
            if (fromInternet)
                return await UpdateFromInternetAsync(fallbackOnError, cancellationToken);
            else
                return await UpdateFromOfflineFileAsync(cancellationToken);
        }
        catch (Exception exception)
        {
            _logger.LogError(
                exception,
                "An error occurred while updating countries and states. Message: {Message}",
                exception.Message);

            return Result.Failure(CountryErrors.UpdateFailed);
        }
    }

    private async Task<Result> UpdateFromInternetAsync(bool fallbackOnError, CancellationToken cancellationToken = default)
    {
        _logger.LogTrace("Updating countries from the internet");
        using HttpClient client = _httpClientFactory.CreateClient();
        string apiEndpoint = "https://countriesnow.space/api/v0.1/countries/states";

        try
        {
            var watch = Stopwatch.StartNew();
            var response = await client.GetFromJsonAsync<CountryStateApiResponse>(apiEndpoint, cancellationToken);
            watch.Stop();

            if (response is null)
            {
                _logger.LogError("Cannot update countries. API response was null or empty");
                if (fallbackOnError)
                    return await UpdateFromOfflineFileAsync(cancellationToken);

                return Result.Failure(CountryErrors.UpdateFailed with
                {
                    Description = "API response was null or empty.",
                });
            }

            if (response.HasError)
            {
                _logger.LogError("Cannot update countries. API response indicated an error. Response: {@Response}", response);
                if (fallbackOnError)
                    return await UpdateFromOfflineFileAsync(cancellationToken);

                return Result.Failure(CountryErrors.UpdateFailed with
                {
                    Description = "API response indicated an error.",
                });
            }

            _logger.LogDebug("GET {URI} returned {Count} countries in {Time} ms",
                apiEndpoint,
                response.Countries.Count,
                watch.ElapsedMilliseconds);

            // Asynchronously update the offline file while updating the records in the database.
            Task updateOfflineFileTask = UpdateOfflineFileAsync(response.Countries, cancellationToken);
            await UpdateCountriesInDatabaseAsync(response.Countries, cancellationToken);
            await updateOfflineFileTask;
            _logger.LogInformation("Countries and states updated");

            return Result.Success();
        }
        catch (Exception exception)
        {
            if (fallbackOnError)
            {
                _logger.LogWarning(
                    exception,
                    "Could not update countries using the internet. Falling back to offline file. Message: {Message}",
                    exception.Message);
                return await UpdateFromOfflineFileAsync(cancellationToken);
            }

            _logger.LogError(
                exception,
                "An error occurred while updating countries and states. Message: {Message}",
                exception.Message);
            return Result.Failure(CountryErrors.UpdateFailed);
        }
    }

    private async Task<Result> UpdateFromOfflineFileAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogTrace("Updating countries from offline file");
        bool fileExists = File.Exists(s_filePath);

        if (!fileExists)
        {
            _logger.LogError("Cannot update countries. File '{File}' does not exist", s_filePath);
            return Result.Failure(CountryErrors.UpdateFailed with
            {
                Description = "Offline file does not exist.",
            });
        }

        string fileContent = await File.ReadAllTextAsync(s_filePath, cancellationToken);
        List<CountryApiResponse>? countries = JsonSerializer.Deserialize<List<CountryApiResponse>>(fileContent);

        if (countries is null)
        {
            _logger.LogDebug("Cannot update countries. File content is null, empty or cannot be parsed correctly");
            return Result.Failure(CountryErrors.UpdateFailed with
            {
                Description = "File content is null, empty or cannot be parsed correctly.",
            });
        }

        await UpdateCountriesInDatabaseAsync(countries, cancellationToken);
        _logger.LogInformation("Countries and states updated");

        return Result.Success();
    }

    private async Task UpdateOfflineFileAsync(List<CountryApiResponse> countries, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Updating offline countries file");
        string json = JsonSerializer.Serialize(countries, s_jsonSerializerOptions);
        await File.WriteAllTextAsync(s_filePath, json, cancellationToken);
        _logger.LogDebug("Done updating offline countries file");
    }

    private async Task UpdateCountriesInDatabaseAsync(List<CountryApiResponse> countries, CancellationToken cancellationToken = default)
    {
        _logger.LogTrace("Received {Count} countries to update", countries.Count);

        foreach (CountryApiResponse apiCountry in countries)
        {
            Country country = apiCountry.Adapt<Country>();
            await AddOrUpdateCountryAsync(country, cancellationToken);
        }

        await _unitOfWork.CompleteAsync();
        _logger.LogDebug("{Count} countries and {Count} states added",
            _countriesAdded,
            _statesAdded);
    }

    private async Task AddOrUpdateCountryAsync(Country country, CancellationToken cancellationToken = default)
    {
        Country? dbCountry = await _unitOfWork.Countries.FindByNameAsync(country.Name, cancellationToken);
        if (dbCountry is null)
        {
            _logger.LogTrace("Country '{Country}' does not exist in database. Now inserting it", country.Name);

            await _unitOfWork.Countries.AddAsync(country, cancellationToken);

            _countriesAdded++;
            _statesAdded += country.States.Count;

            return;
        }

        _logger.LogTrace("Country '{Country}' already exists. Comparing country's states", country.Name);

        foreach (State state in country.States)
        {
            state.CountryId = dbCountry.Id;
        }

        // Find states that are not in the database and insert them.
        IEnumerable<State> statesNotInDb = country.States.Except(dbCountry.States);
        if (statesNotInDb.Any())
        {
            _logger.LogDebug("{Count} states of country '{Country}' are not in the database. Inserting them",
                statesNotInDb.LongCount(),
                country.Name);


            await _unitOfWork.States.AddRangeAsync(statesNotInDb, cancellationToken);
            _statesAdded += statesNotInDb.LongCount();

            return;
        }

        _logger.LogTrace("All states of country '{Country}' are up to date", country.Name);
    }
}
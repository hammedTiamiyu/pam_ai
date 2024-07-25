using PAMAi.Application.Dto.Country;

namespace PAMAi.Application.Services.Interfaces;

/// <summary>
/// Service for operations on country and states.
/// </summary>
public interface ICountryService
{
    /// <summary>
    /// Get all countries.
    /// </summary>
    /// <param name="cancellationToken">
    /// Token to cancel operation.
    /// </param>
    /// <returns>
    /// A <see cref="Result{T}"/> containing all countries.
    /// </returns>
    Task<Result<List<ReadCountryResponse>>> GetCountriesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Get the states of a country.
    /// </summary>
    /// <param name="countryId">
    /// Country's ID.
    /// </param>
    /// <param name="cancellationToken">
    /// Token to cancel the operation.
    /// </param>
    /// <returns>
    /// A list of the country's states if the country exists.
    /// </returns>
    Task<Result<List<ReadCountryStateResponse>>> GetCountryStatesAsync(int countryId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Update countries and states in the database.
    /// </summary>
    /// <param name="fromInternet">
    /// Indicates if the countries and states should be updated from the internet.
    /// If <see langword="false"/>, will use a file for the updates.
    /// </param>
    /// <param name="fallbackOnError">
    /// Indicates if the update should be done from the offline file if the internet
    /// can't be reached or does not work properly.
    /// </param>
    /// <param name="cancellationToken">
    /// Token to cancel the operation.
    /// </param>
    /// <returns>
    /// The result of the operation.
    /// </returns>
    Task<Result> UpdateCountriesAndStatesAsync(bool fromInternet = true, bool fallbackOnError = true, CancellationToken cancellationToken = default);
}

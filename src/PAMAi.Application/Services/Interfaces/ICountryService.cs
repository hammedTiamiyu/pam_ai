namespace PAMAi.Application.Services.Interfaces;

/// <summary>
/// Service for operations on country and states.
/// </summary>
public interface ICountryService
{
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

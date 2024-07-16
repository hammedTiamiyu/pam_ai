namespace PAMAi.Application.Storage;

/// <summary>
/// Unit of work for handling transactions and concurrency.
/// </summary>
public interface IUnitOfWork: IDisposable, IAsyncDisposable
{
    #region Repositories
    ICountryRepository Countries { get; }
    IStateRepository States { get; }
    IUserProfileRepository UserProfiles { get; }
    #endregion

    /// <summary>
    /// Complete the current transaction. All changes are saved to the database.
    /// </summary>
    /// <returns>
    /// The number of entries written or modified in the database.
    /// </returns>
    Task<int> CompleteAsync(CancellationToken cancellationToken = default);
}

using PAMAi.Application.Storage.Base;
using PAMAi.Domain.Entities;

namespace PAMAi.Application.Storage;

/// <summary>
/// <see cref="TermsOfService"/> entities in the database.
/// </summary>
public interface ITermsOfServiceRepository: IRepository<TermsOfService, int>
{
    /// <summary>
    /// Get the current Terms of Service.
    /// </summary>
    /// <param name="cancellationToken">
    /// Token for cancelling the operation.
    /// </param>
    /// <returns>
    /// Terms of Service if a match is found, otherwise <see langword="null"/>.
    /// </returns>
    Task<TermsOfService?> GetCurrentAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Check if the given version is currently in use by another <see cref="TermsOfService"/>.
    /// </summary>
    /// <param name="version">
    /// Version number.
    /// </param>
    /// <param name="cancellationToken">
    /// Token for cancelling the operation.
    /// </param>
    /// <returns>
    /// <see langword="true"/> if the version is used by another <see cref="TermsOfService"/>,
    /// otherwise <see langword="false"/>.
    /// </returns>
    Task<bool> IsVersionInUseAsync(double version, CancellationToken cancellationToken = default);
}

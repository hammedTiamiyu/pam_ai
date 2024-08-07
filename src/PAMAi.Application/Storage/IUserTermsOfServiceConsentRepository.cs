using PAMAi.Application.Storage.Base;
using PAMAi.Domain.Entities;

namespace PAMAi.Application.Storage;

/// <summary>
/// <see cref="UserTermsOfServiceConsent"/> entities in the database.
/// </summary>
public interface IUserTermsOfServiceConsentRepository: IRepository<UserTermsOfServiceConsent>
{
    /// <summary>
    /// Find by user profile and Terms of Service.
    /// </summary>
    /// <param name="userProfileId">
    /// User profile's ID.
    /// </param>
    /// <param name="termsOfServiceId">
    /// Terms of Service's ID.
    /// </param>
    /// <param name="cancellationToken">
    /// Token for cancelling the operation.
    /// </param>
    /// <returns>
    /// The matching record, otherwise <see langword="null"/>.
    /// </returns>
    Task<UserTermsOfServiceConsent?> FindAsync(Guid userProfileId, int termsOfServiceId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Check if the user has accepted the current Terms of Service.
    /// </summary>
    /// <param name="userProfileId">
    /// User profile's ID.
    /// </param>
    /// <param name="cancellationToken">
    /// Token for cancelling the operation.
    /// </param>
    /// <returns>
    /// <see langword="true"/> if the user accepted the current Terms of Service.
    /// </returns>
    Task<bool> IsCurrentTermsOfServiceAcceptedAsync(Guid userProfileId, CancellationToken cancellationToken = default);
}

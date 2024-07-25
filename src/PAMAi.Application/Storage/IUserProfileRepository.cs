using PAMAi.Application.Storage.Base;
using PAMAi.Domain.Entities;

namespace PAMAi.Application.Storage;

/// <summary>
/// <see cref="UserProfile"/> entities in the database.
/// </summary>
public interface IUserProfileRepository: IRepository<UserProfile, Guid>
{
    /// <summary>
    /// Find a user's profile using their user ID.
    /// </summary>
    /// <param name="userId">User ID.</param>
    /// <param name="cancellationToken">
    /// Token for cancelling the operation.
    /// </param>
    /// <returns>
    /// The <see cref="UserProfile"/> if there was a match, otherwise <see langword="null"/>.
    /// </returns>
    Task<UserProfile?> FindAsync(string userId, CancellationToken cancellationToken = default);
}

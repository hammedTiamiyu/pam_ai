using PAMAi.Application.Storage.Base;
using PAMAi.Domain.Entities;

namespace PAMAi.Application.Storage;

/// <summary>
/// <see cref="UserProfile"/> entities in the database.
/// </summary>
public interface IUserProfileRepository: IRepository<UserProfile, Guid>
{
}

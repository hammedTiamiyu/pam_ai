using PAMAi.Application.Storage.Base;
using PAMAi.Domain.Entities;

namespace PAMAi.Application.Storage;

/// <summary>
/// <see cref="State"/> entities in the database.
/// </summary>
public interface IAssetRepository: IRepository<Asset, Guid>
{
}

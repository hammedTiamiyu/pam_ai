using PAMAi.Application.Dto;
using PAMAi.Application.Dto.Parameters;
using PAMAi.Application.Storage.Base;
using PAMAi.Domain.Entities;

namespace PAMAi.Application.Storage;

/// <summary>
/// <see cref="State"/> entities in the database.
/// </summary>
public interface IAssetRepository: IRepository<Asset, Guid>
{
    /// <summary>
    /// Get assets created by installer.
    /// </summary>
    /// <param name="installerId">Installer ID.</param>
    /// <param name="paginationParams">Pagination parameters.</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<PagedList<Asset>> GetAsync(string installerId, PaginationParameters paginationParams, CancellationToken cancellationToken = default);

    /// <summary>
    /// Indicates if the user has multiple assets.
    /// </summary>
    /// <param name="userId">
    /// User ID.
    /// </param>
    /// <param name="cancellationToken">
    /// Token to cancel the operation.
    /// </param>
    /// <returns></returns>
    Task<bool> UserHasMultipleAssetsAsync(string userId, CancellationToken cancellationToken = default);
}

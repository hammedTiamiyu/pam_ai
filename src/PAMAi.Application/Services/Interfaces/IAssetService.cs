using PAMAi.Application.Dto;
using PAMAi.Application.Dto.Asset;
using PAMAi.Application.Dto.Parameters;

namespace PAMAi.Application.Services.Interfaces;

/// <summary>
/// Service for asset creation and management.
/// </summary>
public interface IAssetService
{
    /// <summary>
    /// Create asset.
    /// </summary>
    /// <param name="asset">Asset.</param>
    /// <param name="cancellationToken">
    /// Token for cancelling the operation.
    /// </param>
    /// <returns>
    /// The result of the operation containing the ID of the newly-created asset.
    /// </returns>
    Task<Result<Guid>> CreateAsync(CreateAssetRequest asset, CancellationToken cancellationToken = default);

    /// <summary>
    /// Delete an asset.
    /// </summary>
    /// <param name="id">
    /// Asset ID.
    /// </param>
    /// <param name="cancellationToken">
    /// Token for cancelling the operation.
    /// </param>
    /// <returns>
    /// The result of the operation.
    /// </returns>
    Task<Result> DeleteAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get some assets.
    /// </summary>
    /// <param name="paginationParameters">
    /// Parameters for pagination.
    /// </param>
    /// <param name="cancellationToken">
    /// Token for cancelling the operation.
    /// </param>
    /// <returns>
    /// The result of the operation.
    /// </returns>
    Task<Result<PagedList<PreviewAssetResponse>>> GetAsync(PaginationParameters paginationParameters, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get an asset.
    /// </summary>
    /// <param name="id">
    /// Asset's ID.
    /// </param>
    /// <param name="cancellationToken">
    /// Token for cancelling the operation.
    /// </param>
    /// <returns>
    /// The asset if the result is successful.
    /// </returns>
    Task<Result<ReadAssetResponse>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Send account details to asset user.
    /// </summary>
    /// <param name="assetId">Asset ID.</param>
    /// <param name="cancellationToken">
    /// Token for cancelling the operation.
    /// </param>
    /// <returns>
    /// The result of the operation.
    /// </returns>
    Task<Result> InviteAssetUserAsync(Guid assetId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Update asset.
    /// </summary>
    /// <param name="id">
    /// Asset ID.
    /// </param>
    /// <param name="asset">
    /// Updated asset.
    /// </param>
    /// <param name="cancellationToken">
    /// Token to cancel the operation.
    /// </param>
    /// <returns>
    /// Updated asset.
    /// </returns>
    Task<Result<ReadAssetResponse>> UpdateAsync(Guid id, UpdateAssetRequest asset, CancellationToken cancellationToken = default);
}

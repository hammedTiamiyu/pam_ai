using PAMAi.Application.Dto;
using PAMAi.Application.Dto.Asset;
using PAMAi.Application.Dto.Parameters;
using PAMAi.Application.Services.Interfaces;

namespace PAMAi.API.Controllers;

/// <summary>
/// Manage assets and their owners
/// </summary>
[ApiController]
[ApiVersion(1.0)]
[RequiresRoles("Installer")]
[Route("api/v{version:apiVersion}/[controller]")]
public class AssetsController: BaseController
{
    private readonly IAssetService _assetService;

    public AssetsController(IHttpContextAccessor httpContextAccessor, IAssetService assetService)
        : base(httpContextAccessor)
    {
        _assetService = assetService;
    }

    /// <summary>
    /// Get assets
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(PagedList<PreviewAssetResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAsync([FromQuery] PaginationParameters paginationParameters, CancellationToken cancellationToken)
    {
        var result = await _assetService.GetAsync(paginationParameters, cancellationToken);

        return result.Match(
            onSuccess: Ok,
            onFailure: ErrorResult);
    }

    /// <summary>
    /// Create asset
    /// </summary>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<IActionResult> CreateAsync(CreateAssetRequest asset, CancellationToken cancellationToken)
    {
        var result = await _assetService.CreateAsync(asset, cancellationToken);

        return result.Match(
            onSuccess: data => CreatedAtRoute("GetAssetById", new { id = result.Data }, null),
            onFailure: ErrorResult);
    }

    /// <summary>
    /// Get an asset by its ID
    /// </summary>
    /// <param name="id">Asset ID</param>
    [HttpGet("{id:guid}", Name = "GetAssetById")]
    [ProducesResponseType(typeof(ReadAssetResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var result = await _assetService.GetByIdAsync(id, cancellationToken);

        return result.Match(
            onSuccess: Ok,
            onFailure: ErrorResult);
    }

    /// <summary>
    /// Update an asset
    /// </summary>
    /// <param name="id">Asset ID</param>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(ReadAssetResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateAsync(Guid id, UpdateAssetRequest asset, CancellationToken cancellationToken)
    {
        var result = await _assetService.UpdateAsync(id, asset, cancellationToken);

        return result.Match(
            onSuccess: Ok,
            onFailure: ErrorResult);
    }

    /// <summary>
    /// Delete an asset
    /// </summary>
    /// <param name="id">Asset ID</param>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var result = await _assetService.DeleteAsync(id, cancellationToken);

        return result.Match(
            onSuccess: NoContent,
            onFailure: ErrorResult);
    }

    /// <summary>
    /// Invite the asset's user to PAMAi
    /// </summary>
    /// <param name="id">Asset ID</param>
    [HttpPost("{id:guid}/user/invite")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> InviteAssetUserAsync(Guid id, CancellationToken cancellationToken)
    {
        var result = await _assetService.InviteAssetUserAsync(id, cancellationToken);

        return result.Match(
            onSuccess: NoContent,
            onFailure: ErrorResult);
    }
}

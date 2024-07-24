using System.Diagnostics;
using Microsoft.Extensions.Logging;
using PAMAi.Application.Dto;
using PAMAi.Application.Dto.Parameters;
using PAMAi.Application.Storage;
using PAMAi.Infrastructure.Storage.Contexts;
using PAMAi.Infrastructure.Storage.Repositories.Base;

namespace PAMAi.Infrastructure.Storage.Repositories;

internal class AssetRepository: Repository<Asset, Guid>, IAssetRepository
{
    public AssetRepository(ApplicationDbContext context, ILogger<UnitOfWork> logger)
        : base(context, logger)
    {
    }

    public async Task<PagedList<Asset>> GetAsync(
        string installerId,
        PaginationParameters paginationParams,
        CancellationToken cancellationToken = default)
    {
        Logger.LogTrace(
            "Retrieving {entity} records created by installer {InstallerId} (page = {page}, page size = {size})",
            nameof(Asset),
            installerId,
            paginationParams.Page,
            paginationParams.PageSize);

        var watch = Stopwatch.StartNew();
        PagedList<Asset> entities = await DbContext.Assets
            .Where(a => EF.Functions.Like(a.InstallerId, installerId))
            .ToPagedListAsync(paginationParams.Page, paginationParams.PageSize, cancellationToken);
        watch.Stop();
        Logger.LogDebug("Retrieved paged list of {Entity} records created by installer {InstallerId} in {Time} ms. Parameters: {@Params}",
            nameof(Asset),
            installerId,
            watch.ElapsedMilliseconds,
            paginationParams);

        return entities;
    }

    public async Task<bool> UserHasMultipleAssetsAsync(string userId, CancellationToken cancellationToken = default)
    {
        int assetsOwned = await DbContext.Assets
            .Where(a => a.OwnerId.ToLower() == userId.ToLower())
            .CountAsync(cancellationToken);

        Logger.LogDebug("User {Id} has {Count} assets", userId, assetsOwned);

        return assetsOwned > 1;
    }
}

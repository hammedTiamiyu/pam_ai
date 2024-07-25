using System.Diagnostics;
using Microsoft.Extensions.Logging;
using PAMAi.Application.Dto;
using PAMAi.Application.Dto.Parameters;
using PAMAi.Application.Storage;
using PAMAi.Domain.Entities.Base;
using PAMAi.Infrastructure.Storage.Contexts;
using PAMAi.Infrastructure.Storage.Repositories.Base;

namespace PAMAi.Infrastructure.Storage.Repositories;

internal class AssetRepository: Repository<Asset, Guid>, IAssetRepository
{
    public AssetRepository(ApplicationDbContext context, ILogger<UnitOfWork> logger)
        : base(context, logger)
    {
    }

    public override async Task<Asset?> FindAsync(Guid id, CancellationToken cancellationToken = default)
    {
        Logger.LogTrace("Finding {entity} record by key {key}", nameof(Asset), id);

        var watch = Stopwatch.StartNew();
        Asset? entity = await DbContext.Assets
            .Where(a => EF.Functions.Like(a.Id.ToString(), id.ToString()))
            .Include(a => a.InstallerProfile)
            .Include(a => a.OwnerProfile)
            .FirstOrDefaultAsync(cancellationToken);

        watch.Stop();
        Logger.LogDebug(
            "Searched {entity} records for entity with key {key} in {time} ms. Found: {found}",
            nameof(Asset),
            id,
            watch.ElapsedMilliseconds,
            entity is not null);

        return entity;
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
            .Where(a => EF.Functions.Like(a.InstallerProfile.UserId, installerId))
            .Include(a => a.OwnerProfile)
            .ToPagedListAsync(paginationParams.Page, paginationParams.PageSize, cancellationToken);

        watch.Stop();
        Logger.LogDebug("Retrieved paged list of {Entity} records created by installer {InstallerId} in {Time} ms. Parameters: {@Params}",
            nameof(Asset),
            installerId,
            watch.ElapsedMilliseconds,
            paginationParams);

        return entities;
    }

    public async Task<bool> UserHasMultipleAssetsAsync(Guid userProfileId, CancellationToken cancellationToken = default)
    {
        int assetsOwned = await DbContext.Assets
            .Where(a => EF.Functions.Like(a.OwnerProfileId.ToString(), userProfileId.ToString()))
            .CountAsync(cancellationToken);

        Logger.LogDebug("User {Id} has {Count} assets", userProfileId, assetsOwned);

        return assetsOwned > 1;
    }
}

using System.Diagnostics;
using Microsoft.Extensions.Logging;
using PAMAi.Application.Dto;
using PAMAi.Application.Dto.Parameters;
using PAMAi.Application.Storage;
using PAMAi.Infrastructure.Storage.Contexts;
using PAMAi.Infrastructure.Storage.Repositories.Base;

namespace PAMAi.Infrastructure.Storage.Repositories;
internal class TermsOfServiceRepository: Repository<TermsOfService, int>, ITermsOfServiceRepository
{
    public TermsOfServiceRepository(ApplicationDbContext context, ILogger<UnitOfWork> logger)
        : base(context, logger)
    {
    }

    public override async Task<IEnumerable<TermsOfService>> GetAsync(CancellationToken cancellationToken = default)
    {
        Logger.LogTrace("Retrieving all {entity} records", nameof(TermsOfService));

        var watch = Stopwatch.StartNew();
        // Fetch only the IDs of the profiles (that's what is needed for now).
        var terms = await DbContext.TermsOfService
            .Select(tos => new TermsOfService
            {
                Id = tos.Id,
                Version = tos.Version,
                DeactivatedUtc = tos.DeactivatedUtc,
                EffectiveFromUtc = tos.EffectiveFromUtc,
            })
            .ToListAsync(cancellationToken);
        watch.Stop();
        Logger.LogDebug("Retrieved all {entity} records in {time} ms", nameof(TermsOfService), watch.ElapsedMilliseconds);

        return terms;
    }

    public override async Task<PagedList<TermsOfService>> GetAsync(PaginationParameters paginationParams, CancellationToken cancellationToken = default)
    {
        Logger.LogTrace(
            "Retrieving paged list of {entity} records (page = {page}, page size = {size})",
            nameof(TermsOfService),
            paginationParams.Page,
            paginationParams.PageSize);

        var watch = Stopwatch.StartNew();
        // Order them by their version in descending order.
        var terms = await DbContext.TermsOfService
            .OrderByDescending(tos => tos.Version)
            .ToPagedListAsync(paginationParams.Page, paginationParams.PageSize, cancellationToken);
        watch.Stop();
        Logger.LogDebug("Retrieved {Entity} records in {Time} ms. Parameters: {@Params}",
            nameof(TermsOfService),
            watch.ElapsedMilliseconds,
            paginationParams);

        return terms;
    }

    public async Task<TermsOfService?> GetCurrentAsync(CancellationToken cancellationToken = default)
    {
        Logger.LogTrace("Retrieving current Terms of Service");

        var watch = Stopwatch.StartNew();
        // The current Term of Service is the one which is effective and isn't deactivated.
        TermsOfService? term = await GetCurrentTermsOfServiceAsync(DbContext, cancellationToken);
        watch.Stop();
        Logger.LogDebug("Retrieved the current Term of Service record in {Time} ms", watch.ElapsedMilliseconds);

        return term;
    }

    public async Task<bool> IsVersionInUseAsync(double version, CancellationToken cancellationToken = default)
    {
        Logger.LogTrace("Checking if any Terms of Service is in version {Version}", version);

        var watch = Stopwatch.StartNew();
        bool isInUse = await DbContext.TermsOfService
            .Where(t => t.Version == version)
            .AnyAsync(cancellationToken);
        watch.Stop();
        Logger.LogDebug("Checked if version {Version} is in use by any Terms of Service in {Time} ms. In use: {Value}",
            version,
            watch.ElapsedMilliseconds,
            isInUse);

        return isInUse;
    }

    internal static Task<TermsOfService?> GetCurrentTermsOfServiceAsync(ApplicationDbContext dbContext, CancellationToken cancellationToken)
    {
        return dbContext.TermsOfService
            .Where(t => t.DeactivatedUtc == null)
            .Where(t => DateTimeOffset.UtcNow >= t.EffectiveFromUtc)
            .OrderByDescending(t => t.Version)
            .FirstOrDefaultAsync(cancellationToken);
    }
}

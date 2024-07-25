using PAMAi.Application.Dto;
using PAMAi.Domain.Entities.Base;

namespace PAMAi.Infrastructure.Storage.Extensions;

internal static class IQueryableExtensions
{
    internal static async Task<PagedList<T>> ToPagedListAsync<T>(this IQueryable<T> values,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
        where T : class, IEntity
    {
        // Count as split query if possible.
        long count = await values
            .AsSplitQuery()
            .LongCountAsync(cancellationToken);

        // Skip and take as a single query.
        // We use a single query here because a split query would first ignore the skip and take for the
        // first query run.
        var items = await values
            .AsSingleQuery()
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        if (cancellationToken.IsCancellationRequested)
            throw new TaskCanceledException("Operation has been cancelled.");

        return new PagedList<T>(items, count, page, pageSize);
    }
}
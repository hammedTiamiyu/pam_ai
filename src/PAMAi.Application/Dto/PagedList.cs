using Microsoft.EntityFrameworkCore;

namespace PAMAi.Application.Dto;

/// <summary>
/// Represents a paged list of <typeparamref name="T"/> items.
/// </summary>
public sealed class PagedList<T>: List<T> where T : class
{
    /// <summary>
    /// Current page returned.
    /// </summary>
    public int CurrentPage { get; private set; }

    /// <summary>
    /// Number of distinct pages available for the given page size.
    /// </summary>
    public int TotalPages { get; private set; }

    /// <summary>
    /// Size of the current page.
    /// </summary>
    public int PageSize { get; private set; }

    /// <summary>
    /// Total number of records of <typeparamref name="T"/>.
    /// </summary>
    public long TotalCount { get; private set; }

    /// <summary>
    /// Flag indicating if there is a page that comes before the current page.
    /// </summary>
    public bool HasPrevious => CurrentPage > 1;

    /// <summary>
    /// Indicates if there is a page that comes after the current page.
    /// </summary>
    public bool HasNext => CurrentPage < TotalPages;

    /// <summary>
    /// Creates a new instance of <see cref="PagedList{T}"/>.
    /// </summary>
    /// <param name="items">Items to add to the list.</param>
    /// <param name="totalCount">Total count of items <typeparamref name="T"/>.</param>
    /// <param name="currentPage">Current page.</param>
    /// <param name="pageSize">Size of the current page.</param>
    internal PagedList(List<T> items, long totalCount, int currentPage, int pageSize)
    {
        TotalCount = totalCount;
        PageSize = pageSize;
        CurrentPage = currentPage;
        TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
        AddRange(items);
    }

    /// <summary>
    /// Create a <see cref="PagedList{T}"/> of a data using the given page and page size.
    /// </summary>
    /// <param name="source">Data source.</param>
    /// <param name="pageNumber">Page number.</param>
    /// <param name="pageSize">Page size.</param>
    /// <returns>A <see cref="PagedList{T}"/>.</returns>
    public static async Task<PagedList<T>> CreateAsync(
        IQueryable<T> source,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        // Count as split query if possible.
        long count = await source
            .AsSplitQuery()
            .LongCountAsync(cancellationToken);

        // Skip and take as a single query.
        // We use a single query here because a split query would first ignore the skip and take for the
        // first query run.
        var items = await source
            .AsSingleQuery()
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        if (cancellationToken.IsCancellationRequested)
            throw new TaskCanceledException("Operation has been cancelled.");

        return new PagedList<T>(items, count, pageNumber, pageSize);
    }
}
namespace PAMAi.Application.Dto;

/// <summary>
/// Represents a paged list of <typeparamref name="T"/> items.
/// </summary>
public sealed class PagedList<T>
{
    /// <summary>
    /// Creates a new instance of <see cref="PagedList{T}"/>.
    /// </summary>
    /// <param name="items">Items to add to the list.</param>
    /// <param name="totalCount">Total count of items <typeparamref name="T"/>.</param>
    /// <param name="currentPage">Current page.</param>
    /// <param name="pageSize">Size of the current page.</param>
    public PagedList(List<T> items, long totalCount, int currentPage, int pageSize)
    {
        Data = items;
        Metadata = new PagedListMetadata(totalCount, currentPage, pageSize);
    }

    /// <summary>
    /// Paged list items.
    /// </summary>
    public IReadOnlyList<T> Data { get; }

    /// <summary>
    /// Metadata containing information about the paged list.
    /// </summary>
    public PagedListMetadata Metadata { get; }

    /// <summary>
    /// Transform this paged list to a paged list of <typeparamref name="U"/>
    /// by using Mapster.
    /// </summary>
    /// <typeparam name="U">
    /// Destination type.
    /// </typeparam>
    /// <returns>
    /// The transformed paged list.
    /// </returns>
    internal PagedList<U> Adapt<U>()
    {
        List<U> data = Data.Adapt<List<U>>();
        return Clone(data);
    }

    /// <summary>
    /// Transform this paged list to a paged list of <typeparamref name="U"/>
    /// by using Mapster.
    /// </summary>
    /// <typeparam name="U">
    /// Destination type.
    /// </typeparam>
    /// <param name="config">
    /// Mapster <see cref="TypeAdapterConfig"/> to use in transforming the items
    /// of this paged list.
    /// </param>
    /// <returns>
    /// The transformed paged list.
    /// </returns>
    internal PagedList<U> Adapt<U>(TypeAdapterConfig config)
    {
        List<U> data = Data.Adapt<List<U>>(config);
        return Clone(data);
    }

    private PagedList<U> Clone<U>(List<U> items)
    {
        return new PagedList<U>(items, Metadata.TotalCount, Metadata.CurrentPage, Metadata.PageSize);
    }

    /// <summary>
    /// Metadata of a paged list.
    /// </summary>
    public class PagedListMetadata
    {
        public PagedListMetadata(long totalCount, int currentPage, int pageSize)
        {
            TotalCount = totalCount;
            PageSize = pageSize;
            CurrentPage = currentPage;
            TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
        }

        /// <summary>
        /// Current page returned.
        /// </summary>
        public int CurrentPage { get; }

        /// <summary>
        /// Number of distinct pages available for the given page size.
        /// </summary>
        public int TotalPages { get; }

        /// <summary>
        /// Size of the current page.
        /// </summary>
        public int PageSize { get; }

        /// <summary>
        /// Total number of records of <typeparamref name="T"/>.
        /// </summary>
        public long TotalCount { get; }

        /// <summary>
        /// Flag indicating if there is a page that comes before the current page.
        /// </summary>
        public bool HasPrevious => CurrentPage > 1;

        /// <summary>
        /// Indicates if there is a page that comes after the current page.
        /// </summary>
        public bool HasNext => CurrentPage < TotalPages;
    }
}

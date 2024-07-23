namespace PAMAi.Application.Dto;

/// <summary>
/// Paged list of <typeparamref name="T"/>.
/// </summary>
/// <typeparam name="T"></typeparam>
public record PagedListResponse<T>
{
    /// <summary>
    /// Data.
    /// </summary>
    public List<T> Data { get; set; } = [];

    /// <summary>
    /// Metadata.
    /// </summary>
    public PagedListResponseMetadata Metadata { get; set; } = new();

    internal static TypeAdapterConfig FromPagedList<U>()
        where U : class
    {
        return TypeAdapterConfig<PagedList<U>, PagedListResponse<T>>
            .NewConfig()
            .Map(dest => dest.Metadata.CurrentPage, src => src.CurrentPage)
            .Map(dest => dest.Metadata.TotalPages, src => src.TotalPages)
            .Map(dest => dest.Metadata.PageSize, src => src.PageSize)
            .Map(dest => dest.Metadata.TotalCount, src => src.TotalCount)
            .Map(dest => dest.Metadata.HasPrevious, src => src.HasPrevious)
            .Map(dest => dest.Metadata.HasNext, src => src.HasNext)
            .Config;
    }

    public record PagedListResponseMetadata
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
    }
}
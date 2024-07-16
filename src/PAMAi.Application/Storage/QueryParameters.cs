namespace PAMAi.Application.Storage;

/// <summary>
/// Parameters used to search and paginate items from the data source.
/// </summary>
public class QueryParameters
{
    private int _page = 1;
    private int _pageSize = 10;

    /// <summary>
    /// Page to return.
    /// </summary>
    /// <remarks>Default value is 1.</remarks>
    public int Page
    {
        get => _page;
        set => _page = value < 1 ? 1 : value;
    }

    /// <summary>
    /// Size of the page to return.
    /// </summary>
    /// <remarks>Default value is 10.</remarks>
    public int PageSize
    {
        get => _pageSize;
        set => _pageSize = value < 1 ? 10 : value;
    }

    /// <summary>
    /// Search query.
    /// </summary>
    public string? SearchQuery { get; set; }
}

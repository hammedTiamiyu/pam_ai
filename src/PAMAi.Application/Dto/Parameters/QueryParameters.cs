namespace PAMAi.Application.Dto.Parameters;

/// <summary>
/// Parameters used to search and paginate items from the data source.
/// </summary>
public record QueryParameters: PaginationParameters
{
    /// <summary>
    /// Search query.
    /// </summary>
    public string? SearchQuery { get; set; }
}

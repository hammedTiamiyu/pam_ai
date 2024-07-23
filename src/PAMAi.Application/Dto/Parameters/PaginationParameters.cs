namespace PAMAi.Application.Dto.Parameters;

public record PaginationParameters
{
    public int Page { get; set; } = 1;
    public int Size { get; set; } = 10;
}

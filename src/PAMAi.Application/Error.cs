using Microsoft.AspNetCore.Http;

namespace PAMAi.Application;

/// <summary>
/// Error information from an operation.
/// </summary>
/// <param name="Summary">
/// Summary of what happened.
/// </param>
/// <param name="StatusCode">
/// HTTP status code to be returned this error instance.
/// </param>
public sealed record Error(string Summary, int StatusCode = StatusCodes.Status500InternalServerError)
{
    /// <summary>
    /// Empty error indicating that no error occurred during the operation.
    /// </summary>
    public static readonly Error None = new(string.Empty);

    /// <summary>
    /// More description of the error instance.
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Inner error that caused this error to occur.
    /// </summary>
    public Error? InnerError { get; set; }

    public override string ToString() => $"{Summary}";
}

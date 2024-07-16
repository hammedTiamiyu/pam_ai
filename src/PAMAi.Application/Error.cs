namespace PAMAi.Application;

/// <summary>
/// Error information from an operation.
/// </summary>
/// <param name="Code">Short unique code to identify the error.</param>
/// <param name="Summary">
/// Summary of what the error is about.
/// <para>
/// To add extra detail about the problem, use <see cref="ErrorExtensions.AddDetail(Error, string)"/>.
/// </para>
/// </param>
/// <param name="StatusCode">HTTP status code to be returned in the response.</param>
public sealed record Error(string Code, string Summary, int StatusCode = 500)
{
    /// <summary>
    /// Empty error indicating that no error occurred during the operation.
    /// </summary>
    public static readonly Error None = new(string.Empty, string.Empty);

    /// <summary>
    /// Inner error that caused this error to occur.
    /// </summary>
    public Error? InnerError { get; set; }

    /// <summary>
    /// Longer explanation specific to the occurrence of this error. Reading this can help you
    /// easily track and correct the problem.
    /// </summary>
    public string? Detail { get; set; }

    public override string ToString() => $"{Code}: {Summary}";
}

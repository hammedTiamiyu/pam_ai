namespace PAMAi.Application.Extensions;

public static class ErrorExtensions
{
    /// <summary>
    /// Add inner error and return error.
    /// </summary>
    /// <param name="error">Outer error.</param>
    /// <param name="innerError">Inner error.</param>
    /// <returns>The error with its updated detail.</returns>
    public static Error AddInnerError(this Error error, Error innerError)
    {
        error.InnerError = innerError;
        return error;
    }

    /// <summary>
    /// Add detail to the error.
    /// </summary>
    /// <param name="error">Error.</param>
    /// <param name="detail">Detail explaining the specific cause of the problem.</param>
    /// <returns>The error with its updated detail.</returns>
    public static Error AddDetail(this Error error, string detail)
    {
        error.Detail = detail;
        return error;
    }
}

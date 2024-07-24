using Microsoft.AspNetCore.Http;

namespace PAMAi.Application.Errors;

/// <summary>
/// Contains default miscellaneous errors that can be used throughout the
/// application project.
/// </summary>
internal static class DefaultErrors
{
    /// <summary>
    /// Resource or entity not found.
    /// </summary>
    internal static readonly Error NotFound = new("Not found", StatusCodes.Status404NotFound);

    internal static readonly Error Unauthorised = new("Unauthorised", StatusCodes.Status401Unauthorized);
    internal static readonly Error Forbidden = new("Forbidden", StatusCodes.Status403Forbidden);
}

using Microsoft.AspNetCore.Http;

namespace PAMAi.Application.Errors;

internal static class TermsOfServiceErrors
{
    internal static readonly Error DuplicateVersion = new("Cannot create Terms of Service. Version is already in use", StatusCodes.Status409Conflict);
}

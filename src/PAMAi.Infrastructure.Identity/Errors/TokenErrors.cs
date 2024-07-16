using PAMAi.Application;

namespace PAMAi.Infrastructure.Identity.Errors;
internal class TokenErrors
{
    /// <summary>
    /// Token is invalid or expired.
    /// </summary>
    public static readonly Error InvalidOrExpired = new("Token.InvalidOrExpired", "Refresh token is invalid or expired.");
}

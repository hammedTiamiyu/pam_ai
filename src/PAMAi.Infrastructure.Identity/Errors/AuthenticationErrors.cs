using PAMAi.Application;

namespace PAMAi.Infrastructure.Identity.Errors;
internal class AuthenticationErrors
{
    internal static readonly Error LogoutFailed = new("Auth.LogoutFailed", "Logout failed");
}

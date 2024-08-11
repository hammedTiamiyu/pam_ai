using Microsoft.AspNetCore.Http;
using PAMAi.Application;

namespace PAMAi.Infrastructure.Identity.Errors;
internal class AuthenticationErrors
{
    internal static readonly Error ForgotPasswordFailed = new("Forgot password failed");
    internal static readonly Error LoginFailed = new("Login failed", StatusCodes.Status401Unauthorized);
    internal static readonly Error LogoutFailed = new("Logout failed");
    internal static readonly Error RefreshTokenFailed = new("Failed to refresh token", StatusCodes.Status403Forbidden);
    internal static readonly Error ResetPasswordFailed = new("Reset password failed");
}

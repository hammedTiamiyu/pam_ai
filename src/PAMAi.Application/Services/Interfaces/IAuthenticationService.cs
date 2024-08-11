using PAMAi.Application.Dto.Account;
using PAMAi.Application.Dto.Authentication;
using PAMAi.Domain.Enums;

namespace PAMAi.Application.Services.Interfaces;

/// <summary>
/// Service for user authentication.
/// </summary>
public interface IAuthenticationService
{
    /// <summary>
    /// Send a reset-password token to the user to change their password.
    /// </summary>
    /// <param name="request"></param>
    /// <returns>
    /// The result of the operation.
    /// </returns>
    Task<Result<ForgotPasswordResponse>> ForgotPasswordAsync(ForgotPasswordRequest request);

    /// <summary>
    /// Log in user.
    /// </summary>
    /// <param name="credentials">
    /// Account credentials.
    /// </param>
    /// <param name="loginAsRole"></param>
    /// <returns></returns>
    /// <param name="cancellationToken">
    /// Token for cancelling operations.
    /// </param>
    Task<Result<LoginResponse>> LoginAsync(LoginRequest credentials, ApplicationRole loginAsRole, CancellationToken cancellationToken = default);

    /// <summary>
    /// Log in user.
    /// </summary>
    /// <remarks>
    /// Automatically searches the user's roles and logs in using the first role it sees.
    /// </remarks>
    /// <param name="credentials">
    /// Account credentials.
    /// </param>
    /// <param name="cancellationToken">
    /// Token for cancelling operations.
    /// </param>
    /// <returns></returns>
    [Obsolete("Use LoginAsync(LoginRequest, ApplicationRole, CancellationToken) instead.")]
    Task<Result<LoginResponse>> LoginAsync(LoginRequest credentials, CancellationToken cancellationToken = default);

    /// <summary>
    /// Log out user.
    /// </summary>
    /// <param name="accessToken">
    /// Last access token given to user.
    /// </param>
    /// <param name="refreshToken">
    /// Last refresh token given to user.
    /// </param>
    /// <returns></returns>
    Task<Result> LogoutAsync(string accessToken, string refreshToken);

    /// <summary>
    /// Refresh access token.
    /// </summary>
    /// <param name="tokens">Access and refresh tokens.</param>
    /// <param name="cancellationToken">
    /// Token for cancelling the operation.
    /// </param>
    /// <returns></returns>
    Task<Result<RefreshTokenResponse>> RefreshTokenAsync(RefreshTokenRequest tokens, CancellationToken cancellationToken = default);

    /// <summary>
    /// Reset password.
    /// </summary>
    /// <param name="request">
    /// New credentials.
    /// </param>
    /// <returns>
    /// The result of the operation.
    /// </returns>
    Task<Result> ResetPasswordAsync(ResetPasswordRequest request);
}

using PAMAi.Application.Dto.Authentication;

namespace PAMAi.Application.Services.Interfaces;

/// <summary>
/// Service for user authentication.
/// </summary>
public interface IAuthenticationService
{
    /// <summary>
    /// Log in user.
    /// </summary>
    /// <param name="credentials">
    /// Account credentials.
    /// </param>
    /// <param name="cancellationToken">
    /// Token for cancelling operations.
    /// </param>
    /// <returns></returns>
    Task<Result> LoginAsync(LoginRequest credentials, CancellationToken cancellationToken = default);
}

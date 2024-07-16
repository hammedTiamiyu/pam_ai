using PAMAi.Application.Dto.Authentication;
using PAMAi.Domain.Enums;

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
    /// <param name="loginAsRole"></param>
    /// <returns></returns>
    /// <param name="cancellationToken">
    /// Token for cancelling operations.
    /// </param>
    Task<Result> LoginAsync(LoginRequest credentials, ApplicationRole loginAsRole, CancellationToken cancellationToken = default);
}

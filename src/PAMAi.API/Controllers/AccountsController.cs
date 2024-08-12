using PAMAi.Application.Dto.Account;
using PAMAi.Application.Dto.Authentication;
using PAMAi.Application.Services.Interfaces;
using PAMAi.Domain.Enums;

namespace PAMAi.API.Controllers;

/// <summary>
/// Manage account creation and authentication
/// </summary>
[ApiController]
[ApiVersion(1.0)]
[Route("api/v{version:apiVersion}/[controller]")]
public sealed class AccountsController: BaseController
{
    private readonly IActionResult _genericFailedLoginResponse;
    private readonly IAuthenticationService _authenticationService;
    private readonly IAccountService _accountService;

    public AccountsController(
        IHttpContextAccessor httpContextAccessor,
        IAuthenticationService authenticationService,
        IAccountService accountService)
        : base(httpContextAccessor)
    {
        _authenticationService = authenticationService;
        _accountService = accountService;
        _genericFailedLoginResponse = Problem(
            type: "about:blank",
            statusCode: StatusCodes.Status401Unauthorized,
            title: "Unauthorised",
            detail: "Invalid email/username and password.");
    }

    /// <summary>
    /// Account login for users
    /// </summary>
    /// <param name="credential">Account credentials</param>
    [HttpPost("users/login")]
    [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> UserLoginAsync(LoginRequest credential, CancellationToken cancellationToken)
    {
        var result = await _authenticationService.LoginAsync(credential, cancellationToken);

        return result.Match(
            onSuccess: Ok,
            onFailure: error => _genericFailedLoginResponse);
    }

    /// <summary>
    /// Login
    /// </summary>
    /// <param name="credential">
    /// User credentials
    /// </param>
    [HttpPost("login")]
    [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> LoginAsync(NewLoginRequest credential, CancellationToken cancellationToken)
    {
        var result = await _authenticationService.LoginAsync(credential, credential.Role, cancellationToken);

        return result.Match(
            onSuccess: Ok,
            onFailure: error => _genericFailedLoginResponse);
    }

    /// <summary>
    /// Logout
    /// </summary>
    [HttpPost("logout")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> LogoutAsync(LogoutRequest logoutParameters)
    {
        var result = await _authenticationService.LogoutAsync(logoutParameters.AccessToken, logoutParameters.RefreshToken);

        return result.Match(
            onSuccess: NoContent,
            onFailure: ErrorResult);
    }

    /// <summary>
    /// Refresh access token
    /// </summary>
    /// <param name="tokens">Tokens</param>
    [HttpPost("refresh-token")]
    [ProducesResponseType(typeof(RefreshTokenResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> RefreshTokenAsync(RefreshTokenRequest tokens, CancellationToken cancellationToken)
    {
        var result = await _authenticationService.RefreshTokenAsync(tokens, cancellationToken);

        return result.Match(
            onSuccess: Ok,
            onFailure: ErrorResult);
    }

    /// <summary>
    /// Send a link to reset account password
    /// </summary>
    /// <param name="request"></param>
    [HttpPost("forgot-password")]
    [ProducesResponseType(typeof(ForgotPasswordResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> ForgotPasswordAsync(ForgotPasswordRequest request)
    {
        var result = await _authenticationService.ForgotPasswordAsync(request);

        return result.Match(
            onSuccess: Ok,
            onFailure: ErrorResult);
    }

    /// <summary>
    /// Reset account password
    /// </summary>
    /// <param name="credential">
    /// New credential
    /// </param>
    [HttpPost("reset-password")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> ResetPasswordAsync(ResetPasswordRequest credential)
    {
        var result = await _authenticationService.ResetPasswordAsync(credential);

        return result.Match(
            onSuccess: NoContent,
            onFailure: ErrorResult);
    }

    /// <summary>
    /// Create installer account
    /// </summary>
    /// <param name="installer">
    /// Account details
    /// </param>
    [HttpPost("installers")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<IActionResult> CreateInstallerAsync(CreateInstallerRequest installer, CancellationToken cancellationToken)
    {
        var result = await _accountService.CreateInstallerAsync(installer, cancellationToken);

        return result.Match(
            onSuccess: () => CreatedAtRoute(UsersController.GET_LOGGED_IN_USERPROFILE_ROUTE, null),
            onFailure: ErrorResult);
    }

    /// <summary>
    /// Account login for super admins
    /// </summary>
    /// <param name="credential">Account credentials</param>
    [Obsolete("Use LoginAsync() instead.")]
    [HttpPost("admin/login")]
    [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> AdminLoginAsync(LoginRequest credential, CancellationToken cancellationToken)
    {
        var result = await _authenticationService.LoginAsync(credential, ApplicationRole.SuperAdmin, cancellationToken);

        return result.Match(
            onSuccess: Ok,
            onFailure: error => _genericFailedLoginResponse);
    }

    /// <summary>
    /// Account login for installers
    /// </summary>
    /// <param name="credential">Account credentials</param>
    [Obsolete("Use LoginAsync() instead.")]
    [HttpPost("installers/login")]
    [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> InstallerLoginAsync(LoginRequest credential, CancellationToken cancellationToken)
    {
        var result = await _authenticationService.LoginAsync(credential, ApplicationRole.Installer, cancellationToken);

        return result.Match(
            onSuccess: Ok,
            onFailure: error => _genericFailedLoginResponse);
    }
}

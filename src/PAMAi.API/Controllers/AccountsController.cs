using PAMAi.Application.Dto.Account;
using PAMAi.Application.Dto.Authentication;
using PAMAi.Application.Services.Interfaces;
using PAMAi.Domain.Enums;

namespace PAMAi.API.Controllers;

/// <summary>
/// Manage account creation and authentication.
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
    /// Account login for super admins
    /// </summary>
    /// <param name="credential">Account credentials</param>
    [HttpPost("admin/login")]
    [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> LoginAsync(LoginRequest credential, CancellationToken cancellationToken)
    {
        var result = await _authenticationService.LoginAsync(credential, ApplicationRole.SuperAdmin, cancellationToken);

        return result.Match(
            onSuccess: Ok,
            onFailure: error => _genericFailedLoginResponse);
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
    /// Account login for installers
    /// </summary>
    /// <param name="credential">Account credentials</param>
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

    /// <summary>
    /// Create user account
    /// </summary>
    /// <param name="user">
    /// Account details
    /// </param>
    [HttpPost("users")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<IActionResult> CreateUserAsync(CreateUserRequest user, CancellationToken cancellationToken)
    {
        var result = await _accountService.CreateUserAsync(user, cancellationToken);

        return result.Match(
            onSuccess: () => CreatedAtRoute(UsersController.GET_LOGGED_IN_USERPROFILE_ROUTE, null),
            onFailure: ErrorResult);
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
        var result = await _authenticationService.LoginAsync(credential, ApplicationRole.User, cancellationToken);

        return result.Match(
            onSuccess: Ok,
            onFailure: error => _genericFailedLoginResponse);
    }

    /// <summary>
    /// Logout
    /// </summary>
    [HttpPost("logout")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> LogoutAsync(CancellationToken cancellationToken)
    {
        var result = await _authenticationService.LogoutAsync("", "");

        return result.Match(
            onSuccess: NoContent,
            onFailure: ErrorResult);
    }
}

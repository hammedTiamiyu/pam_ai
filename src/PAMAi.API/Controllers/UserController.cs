using PAMAi.Application.Dto.Account;
using PAMAi.Application.Services.Interfaces;

namespace PAMAi.API.Controllers;

/// <summary>
/// Account management
/// </summary>
[ApiController]
[ApiVersion(1.0)]
[RequiresRoles("SuperAdmin, Installer, User")]
[Route("api/v{version:apiVersion}/[controller]")]
public sealed class UsersController: BaseController
{
    internal const string GET_LOGGED_IN_USERPROFILE_ROUTE = "GetLoggedInUserProfile";
    private readonly IAccountService _accountService;

    public UsersController(IHttpContextAccessor httpContextAccessor, IAccountService accountService)
        : base(httpContextAccessor)
    {
        _accountService = accountService;
    }

    /// <summary>
    /// Get profile of the current signed-in user
    /// </summary>
    [HttpGet("me", Name = GET_LOGGED_IN_USERPROFILE_ROUTE)]
    [ProducesResponseType(typeof(ReadProfileResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetLoggedInUserProfileAsync(CancellationToken cancellationToken)
    {
        var result = await _accountService.GetProfileAsync(cancellationToken);

        return result.Match(
            onSuccess: Ok,
            onFailure: ErrorResult);
    }

    /// <summary>
    /// Update profile of the current signed-in user
    /// </summary>
    /// <param name="profile">
    /// Updated profile
    /// </param>
    [HttpPut("me")]
    [ProducesResponseType(typeof(ReadProfileResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdateLoggedInUserProfileAsync(UpdateProfileRequest profile, CancellationToken cancellationToken)
    {
        var result = await _accountService.UpdateProfileAsync(profile, cancellationToken);

        return result.Match(
            onSuccess: Ok,
            onFailure: ErrorResult);
    }

    /// <summary>
    /// Change the password of the current signed-in user
    /// </summary>
    /// <param name="credentials">
    /// New credentials
    /// </param>
    [HttpPost("me/change-password")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> ChangeLoggedInUserPasswordAsync(ChangePasswordRequest credentials, CancellationToken cancellationToken)
    {
        var result = await _accountService.ChangePasswordAsync(credentials, cancellationToken);

        return result.Match(
            onSuccess: NoContent,
            onFailure: ErrorResult);
    }
}

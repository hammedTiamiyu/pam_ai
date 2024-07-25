using PAMAi.Application.Dto.Account;
using PAMAi.Application.Services.Interfaces;

namespace PAMAi.API.Controllers;

/// <summary>
/// Account management
/// </summary>
[ApiController]
[ApiVersion(1.0)]
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
    /// Get profile of the current signed-in user.
    /// </summary>
    [HttpGet("me", Name = GET_LOGGED_IN_USERPROFILE_ROUTE)]
    [RequiresRoles("SuperAdmin,Installer,User")]
    [ProducesResponseType(typeof(ReadProfileResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetLoggedInUserProfileAsync(CancellationToken cancellationToken = default)
    {
        var result = await _accountService.GetProfileAsync(cancellationToken);

        return result.Match(
            onSuccess: Ok,
            onFailure: ErrorResult);
    }
}

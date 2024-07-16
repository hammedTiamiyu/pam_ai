namespace PAMAi.API.Controllers;

/// <summary>
/// View and edit users profiles
/// </summary>
[ApiController]
[ApiVersion(1.0)]
[Route("api/v{version:apiVersion}/[controller]")]
public sealed class UsersController: BaseController
{
    internal const string GET_LOGGED_IN_USERPROFILE_ROUTE = "GetLoggedInUserProfile";

    public UsersController(IHttpContextAccessor httpContextAccessor)
        : base(httpContextAccessor)
    {
    }

    /// <summary>
    /// Get profile of the current signed-in user.
    /// </summary>
    [HttpGet("me", Name = GET_LOGGED_IN_USERPROFILE_ROUTE)]
    public Task<IActionResult> GetLoggedInUserProfileAsync(CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}

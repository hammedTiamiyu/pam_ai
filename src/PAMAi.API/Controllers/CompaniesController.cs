
namespace PAMAi.API.Controllers;

/// <summary>
/// Manage companies
/// </summary>
[ApiController]
[ApiVersion(1.0)]
[Route("api/v{version:apiVersion}/[controller]")]
public sealed class CompaniesController: BaseController
{
    public CompaniesController(IHttpContextAccessor httpContextAccessor)
        : base(httpContextAccessor)
    {
    }
}

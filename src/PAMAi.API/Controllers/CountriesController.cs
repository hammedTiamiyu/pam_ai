using PAMAi.Application.Dto.Country;
using PAMAi.Application.Services.Interfaces;

namespace PAMAi.API.Controllers;

/// <summary>
/// Get countries and states
/// </summary>
[ApiController]
[ApiVersion(1.0)]
[Route("api/v{version:apiVersion}/[controller]")]
public class CountriesController: BaseController
{
    private readonly ICountryService _countryService;

    public CountriesController(IHttpContextAccessor httpContextAccessor, ICountryService countryService) : base(httpContextAccessor)
    {
        _countryService = countryService;
    }

    /// <summary>
    /// Get all countries
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(List<ReadCountryResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAsync(CancellationToken cancellationToken)
    {
        Result<List<ReadCountryResponse>> result = await _countryService.GetCountriesAsync(cancellationToken);
        return result.Match(
            onSuccess: Ok,
            onFailure: ErrorResult);
    }

    /// <summary>
    /// Get country's states
    /// </summary>
    /// <param name="countryId">
    /// Country's ID
    /// </param>
    [HttpGet("{countryId:int}/states")]
    [ProducesResponseType(typeof(List<ReadCountryStateResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetCountryStatesAsync(int countryId, CancellationToken cancellationToken = default)
    {
        Result<List<ReadCountryStateResponse>> result = await _countryService.GetCountryStatesAsync(countryId, cancellationToken);
        return result.Match(
            onSuccess: Ok,
            onFailure: ErrorResult);
    }
}

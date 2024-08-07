using PAMAi.Application.Dto;
using PAMAi.Application.Dto.Parameters;
using PAMAi.Application.Dto.TermsOfService;
using PAMAi.Application.Services.Interfaces;

namespace PAMAi.API.Controllers;

/// <summary>
/// View and manage PAMAi's legal contracts
/// </summary>
[ApiController]
[ApiVersion(1.0)]
[Route("api/v{version:apiVersion}/[controller]")]
public class LegalsController: BaseController
{
    private const string GET_TERMS_BY_ID_ROUTE_NAME = "GetTermsById";

    private readonly ILegalService _legalService;

    public LegalsController(IHttpContextAccessor httpContextAccessor, ILegalService legalService)
        : base(httpContextAccessor)
    {
        _legalService = legalService;
    }

    /// <summary>
    /// Get a history of PAMAi's Terms of Service
    /// </summary>
    /// <param name="paginationParams">
    /// Pagination parameters
    /// </param>
    [HttpGet("terms")]
    [ProducesResponseType(typeof(PagedList<PreviewTermsOfServiceResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetTermsOfServiceAsync(
        [FromQuery] PaginationParameters paginationParams,
        CancellationToken cancellationToken)
    {
        var result = await _legalService.GetTermsOfServiceAsync(paginationParams, cancellationToken);

        return result.Match(
            onSuccess: Ok,
            onFailure: ErrorResult);
    }

    /// <summary>
    /// Create Terms of Service
    /// </summary>
    /// <param name="termsOfService">
    /// Terms of Service's details
    /// </param>
    [HttpPost("terms")]
    [RequiresRoles("SuperAdmin")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CreateTermsOfServiceAsync(CreateTermsOfServiceRequest termsOfService, CancellationToken cancellationToken)
    {
        var result = await _legalService.CreateTermsOfServiceAsync(termsOfService, cancellationToken);
        
        return result.Match(
            onSuccess: data => CreatedAtRoute(GET_TERMS_BY_ID_ROUTE_NAME, new { id = data }, null),
            onFailure: ErrorResult);
    }

    /// <summary>
    /// Get PAMAi's latest effective Terms of Service
    /// </summary>
    /// <response code="204">There's no current Terms of Service in use</response>
    [HttpGet("terms/current")]
    [ProducesResponseType(typeof(ReadTermsOfServiceResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> GetCurrentTermsOfServiceAsync(CancellationToken cancellationToken)
    {
        var result = await _legalService.GetCurrentTermsOfServiceAsync(cancellationToken);

        return result.Match(
            onSuccess: Ok,
            onFailure: ErrorResult);
    }

    /// <summary>
    /// Get a Terms of Service by its ID
    /// </summary>
    /// <param name="id">ID</param>
    [HttpGet("terms/{id:int}", Name = GET_TERMS_BY_ID_ROUTE_NAME)]
    [ProducesResponseType(typeof(ReadTermsOfServiceResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetTermsOfServiceByIdAsync(int id, CancellationToken cancellationToken)
    {
        var result = await _legalService.GetTermsOfServiceByIdAsync(id, cancellationToken);

        return result.Match(
            onSuccess: Ok,
            onFailure: ErrorResult);
    }

    /// <summary>
    /// Delete Terms of Service
    /// </summary>
    /// <param name="id">Terms of Service's ID</param>
    [HttpDelete("terms/{id:int}")]
    [RequiresRoles("SuperAdmin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> DeleteTermsOfServiceAsync(int id, CancellationToken cancellationToken)
    {
        var result = await _legalService.DeleteTermsOfServiceAsync(id, cancellationToken);

        return result.Match(
            onSuccess: NoContent,
            onFailure: ErrorResult);
    }

    /// <summary>
    /// Accept Terms of Service
    /// </summary>
    /// <param name="id">
    /// Terms of Service's ID
    /// </param>
    [HttpPost("terms/{id:int}/accept")]
    [RequiresRoles("SuperAdmin, Installer, User")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> AcceptTermsOfServiceAsync(int id, CancellationToken cancellationToken)
    {
        var result = await _legalService.AcceptTermsOfServiceAsync(id, cancellationToken);

        return result.Match(
            onSuccess: NoContent,
            onFailure: ErrorResult);
    }
}

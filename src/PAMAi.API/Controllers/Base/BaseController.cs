namespace PAMAi.API.Controllers.Base;

public abstract class BaseController: ControllerBase
{
    private protected readonly IHttpContextAccessor _httpContextAccessor;

    protected BaseController(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    /// <summary>
    /// Creates an <see cref="ObjectResult"/> object using the <paramref name="error"/>.
    /// </summary>
    /// <remarks>
    /// If the client accepts plain texts ('text/plain'), it returns a plain summary of the error,
    /// otherwise a Problem Detail is returned.
    /// </remarks>
    /// <param name="error"></param>
    /// <returns></returns>
    protected ObjectResult ErrorResult(Error error)
    {
        return Problem(
            type: "about:blank",
            detail: error.Description,
            instance: _httpContextAccessor.HttpContext?.Request.Path,
            statusCode: error.StatusCode,
            title: error.Summary);
    }
}

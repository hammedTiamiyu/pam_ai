using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using PAMAi.Application.Services.Interfaces;

namespace PAMAi.API.Middlewares;

/// <summary>
/// Middleware to check if authorisation JWT is allowed to operate in the API.
/// </summary>
public class JwtSessionMiddleware: IMiddleware
{
    private readonly ILogger<JwtSessionMiddleware> _logger;
    private readonly ITokenService _tokenService;

    public JwtSessionMiddleware(ILogger<JwtSessionMiddleware> logger, ITokenService tokenService)
    {
        _logger = logger;
        _tokenService = tokenService;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        // Check if user is authenticated using the bearer token.
        // If they aren't, go through. Otherwise, check the bearer token for its validity.
        if (context.User.Identity is not ClaimsIdentity { IsAuthenticated: true })
        {
            await next(context);
            return;
        }

        string? jwt = await context.GetTokenAsync(JwtBearerDefaults.AuthenticationScheme, "access_token");

        if (string.IsNullOrWhiteSpace(jwt))
        {
            _logger.LogError("JWT is empty or null.");

            ProblemDetails problemDetails = new()
            {
                Type = "about:blank",
                Title = "Unauthorised",
                Detail = "Invalid or empty access token",
                Instance = context.Request.Path,
                Status = StatusCodes.Status401Unauthorized,
            };
            context.Response.StatusCode = (int)problemDetails.Status;
            await context.Response.WriteAsJsonAsync(problemDetails);

            return;
        }

        var isBlacklisted = await _tokenService.IsBlacklistedAsync(jwt);
        if (isBlacklisted)
        {
            string? userId = context.User?.FindFirstValue(ClaimTypes.NameIdentifier);
            _logger.LogError("User {user} tried to access API using a blacklisted JSON web token. JWT: {id}", userId, jwt);

            ProblemDetails problemDetails = new()
            {
                Type = "about:blank",
                Title = "Unauthorised",
                Detail = "Invalid or empty access token",
                Instance = context.Request.Path,
                Status = StatusCodes.Status401Unauthorized,
            };
            context.Response.StatusCode = (int)problemDetails.Status;
            await context.Response.WriteAsJsonAsync(problemDetails);

            return;
        }

        await next(context);
    }
}
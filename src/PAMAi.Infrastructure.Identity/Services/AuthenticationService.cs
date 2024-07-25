using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using Mapster;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PAMAi.Application;
using PAMAi.Application.Dto.Authentication;
using PAMAi.Application.Extensions;
using PAMAi.Application.Services.Interfaces;
using PAMAi.Application.Services.Models;
using PAMAi.Domain.Enums;
using PAMAi.Domain.Options;
using PAMAi.Infrastructure.Identity.Errors;
using PAMAi.Infrastructure.Identity.Models;
using IAuthenticationService = PAMAi.Application.Services.Interfaces.IAuthenticationService;

namespace PAMAi.Infrastructure.Identity.Services;

/// <inheritdoc cref="IAuthenticationService"/>
internal sealed class AuthenticationService: IAuthenticationService
{
    private readonly UserManager<User> _userManager;
    private readonly ILogger<AuthenticationService> _logger;
    private readonly ITokenService _tokenService;
    private readonly IdentityContext _identityContext;
    private readonly JwtOptions _jwtOptions;

    public AuthenticationService(
        UserManager<User> userManager,
        ILogger<AuthenticationService> logger,
        ITokenService tokenService,
        IdentityContext identityContext,
        IOptionsSnapshot<JwtOptions> jwtOptionsSnapshot)
    {
        _userManager = userManager;
        _logger = logger;
        _tokenService = tokenService;
        _identityContext = identityContext;
        _jwtOptions = jwtOptionsSnapshot.Value;
    }

    public async Task<Result<LoginResponse>> LoginAsync(
        LoginRequest credentials,
        ApplicationRole loginAsRole,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("'{Username}' is attempting to log in as a {Role}", credentials.Username, loginAsRole);

        User? user = await _userManager.FindByEmailAsync(credentials.Username);
        // Try to find user by username if there was no email match.
        user ??= await _userManager.FindByNameAsync(credentials.Username);

        // If user is null despite the search using their username and email.
        if (user is null)
        {
            _logger.LogError("Login failed. No account matches username/email '{Username}'", credentials.Username);
            return Result<LoginResponse>.Failure(AuthenticationErrors.LoginFailed with
            {
                Description = $"No account exists for '{credentials.Username}'.",
            });
        }

        // Check if user is in role.
        bool isInRole = await _userManager.IsInRoleAsync(user, loginAsRole.ToString());
        if (!isInRole)
        {
            _logger.LogError("Login failed. Account '{Username}' is not in {Role} role", credentials.Username, loginAsRole);
            return Result<LoginResponse>.Failure(AuthenticationErrors.LoginFailed with
            {
                Description = $"Account is not in {loginAsRole} role.",
            });
        }

        bool loginSuccessful = await _userManager.CheckPasswordAsync(user, credentials.Password);
        if (!loginSuccessful)
        {
            _logger.LogError("Login failed. '{Username}' attempted to log in with a wrong password", credentials.Username);
            return Result<LoginResponse>.Failure(AuthenticationErrors.LoginFailed with
            {
                Description = "Invalid credentials.",
            });
        }

        ClaimsIdentity claimsIdentity = await GetUserClaimsIdentityAsync(user, loginAsRole);
        LoginResponse response = _tokenService.GenerateToken(claimsIdentity).Adapt<LoginResponse>();
        var createdUtc = DateTimeOffset.UtcNow;

        user.RefreshTokens.Add(new()
        {
            Token = response.RefreshToken.ToSha512Base64UrlEncoding(),
            CreatedUtc = createdUtc,
            ExpiresUtc = createdUtc.AddDays(_jwtOptions.RefreshTokenValidForInDays),
        });

        _identityContext.Update(user);
        await _identityContext.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("'{Username}' logged in successfully", credentials.Username);

        return Result<LoginResponse>.Success(response);
    }

    public async Task<Result> LogoutAsync(string accessToken, string refreshToken)
    {
        // First, ensure the JWT was created and signed by this application.
        bool hasValidSignature = await _tokenService.ValidateTokenSignatureAsync(accessToken);
        if (!hasValidSignature)
        {
            _logger.LogWarning("An attempt to logout was made with an invalid access token. Info: {@info}",
                new
                {
                    Token = accessToken,
                    RefreshToken = refreshToken,
                });
            return Result.Failure(AuthenticationErrors.LogoutFailed with
            {
                Description = "Invalid access token.",
            });
        }

        User? user = await GetUserByRefreshTokenAsync(refreshToken);
        if (user is null)
        {
            _logger.LogInformation("Logout failed. Refresh token {Token} does not match any user", refreshToken);

            return Result.Failure(AuthenticationErrors.LogoutFailed with
            {
                Description = "Invalid refresh token.",
            });
        }

        user = MarkRefreshTokenAsRevoked(user, refreshToken);
        await _tokenService.BlacklistJwtAsync(accessToken);
        _identityContext.Update(user);
        await _identityContext.SaveChangesAsync();
        _logger.LogInformation("User {Email} logged out", user.Email);

        return Result.Success();
    }

    public async Task<Result<RefreshTokenResponse>> RefreshTokenAsync(RefreshTokenRequest tokens, CancellationToken cancellationToken = default)
    {
        // First, ensure the JWT was created and signed by this application.
        bool hasValidSignature = await _tokenService.ValidateTokenSignatureAsync(tokens.AccessToken, cancellationToken);
        if (!hasValidSignature)
        {
            _logger.LogWarning("An attempt to logout was made with an invalid access token. Info: {@info}",
                new
                {
                    Token = tokens.AccessToken,
                    RefreshToken = tokens.RefreshToken,
                });
            return Result<RefreshTokenResponse>.Failure(AuthenticationErrors.RefreshTokenFailed with
            {
                Description = "Invalid access token.",
            });
        }

        User? user = await GetUserByRefreshTokenAsync(tokens.RefreshToken);
        if (user is null)
        {
            _logger.LogInformation("Logout failed. Refresh token {Token} does not match any user", tokens.RefreshToken);

            return Result<RefreshTokenResponse>.Failure(AuthenticationErrors.RefreshTokenFailed with
            {
                Description = "Invalid refresh token.",
            });
        }

        RefreshToken refreshToken = user.RefreshTokens
            .First(r => r.Token == tokens.RefreshToken.ToSha512Base64UrlEncoding());

        ApplicationRole role = GetRoleFromJwt(tokens.AccessToken);
        ClaimsIdentity claimsIdentity = await GetUserClaimsIdentityAsync(user, role);
        RefreshTokenResponse response = _tokenService.GenerateToken(claimsIdentity).Adapt<RefreshTokenResponse>();
        var createdUtc = DateTimeOffset.UtcNow;

        user.RefreshTokens.Remove(refreshToken);
        user.RefreshTokens.Add(new()
        {
            Token = response.RefreshToken.ToSha512Base64UrlEncoding(),
            CreatedUtc = createdUtc,
            ExpiresUtc = createdUtc.AddDays(_jwtOptions.RefreshTokenValidForInDays),
        });

        _identityContext.Update(user);
        await _identityContext.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("'{Username}' refreshed their access token successfully", user.Email);

        return Result<RefreshTokenResponse>.Success(response);
    }

    private static User MarkRefreshTokenAsRevoked(User user, string refreshToken)
    {
        string hashed = refreshToken.ToSha512Base64UrlEncoding();

        user.RefreshTokens
            .First(r => r.Token.Equals(hashed, StringComparison.OrdinalIgnoreCase))
            .RevokedUtc = DateTimeOffset.UtcNow;

        return user;
    }

    private ApplicationRole GetRoleFromJwt(string jwt)
    {
        JwtSecurityToken securityToken = _tokenService.GetJwtSecurityToken(jwt);
        object role = securityToken.Payload
            .First(claim => string.Equals(claim.Key, ClaimTypes.Role, StringComparison.OrdinalIgnoreCase))
            .Value;
        _logger.LogTrace("Current role as seen in JWT: {Role}", role);

        return Enum.Parse<ApplicationRole>(role?.ToString() ?? string.Empty);
    }

    private async Task<User?> GetUserByRefreshTokenAsync(string refreshToken)
    {
        string hashed = refreshToken.ToSha512Base64UrlEncoding();

        User? user = await _identityContext.Users
            .Where(u => u.RefreshTokens.Any(r => EF.Functions.Like(r.Token, hashed)))
            .Include(u => u.RefreshTokens)
            .FirstOrDefaultAsync();

        _logger.LogTrace("Refresh token {Token} does not match any user", refreshToken);

        return user;
    }

    private async Task<ClaimsIdentity> GetUserClaimsIdentityAsync(User user, ApplicationRole role)
    {
        IList<Claim> userClaims = await _userManager.GetClaimsAsync(user);
        IEnumerable<Claim> claims = new List<Claim>()
        {
            new(ClaimTypes.NameIdentifier, user.Id),
            new(ClaimTypes.Role, role.ToString()),
        }.Union(userClaims);

        return new ClaimsIdentity(claims, JwtBearerDefaults.AuthenticationScheme);
    }
}

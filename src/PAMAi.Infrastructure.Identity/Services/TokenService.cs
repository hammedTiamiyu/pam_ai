using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using PAMAi.Application.Extensions;
using PAMAi.Application.Helpers;
using PAMAi.Application.Services.Interfaces;
using PAMAi.Application.Services.Models;
using PAMAi.Domain.Options;
using PAMAi.Infrastructure.Identity.Models;

namespace PAMAi.Infrastructure.Identity.Services;
internal class TokenService: ITokenService
{
    private readonly JwtOptions _jwtOptions;
    private readonly ILogger<TokenService> _logger;
    private readonly IDistributedCache _distributedCache;
    private readonly UserManager<User> _userManager;

    public TokenService(
        IOptionsSnapshot<JwtOptions> jwtOptionsSnapshot,
        ILogger<TokenService> logger,
        IDistributedCache distributedCache,
        UserManager<User> userManager)
    {
        _jwtOptions = jwtOptionsSnapshot.Value;
        _logger = logger;
        _distributedCache = distributedCache;
        _userManager = userManager;
    }

    public async Task BlacklistJwtAsync(string token, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(token))
            throw new ArgumentException("Token is null or empty.", nameof(token));

        var key = token.ToSha512Base64UrlEncoding();
        _logger.LogTrace("Blacklisting token {Token}. Key: {Key}", token, key);

        var options = new DistributedCacheEntryOptions
        {
            AbsoluteExpiration = DateTimeOffset.Now.AddMinutes(_jwtOptions.ValidForInMinutes)
        };
        await _distributedCache.SetStringAsync(key, token, options, cancellationToken);
        _logger.LogDebug("Token {Token} blacklisted", token);
    }

    public Tokens GenerateToken(ClaimsIdentity claimsIdentity)
    {
        _logger.LogTrace("Generating token for claims identity: {@Claims}", claimsIdentity);

        var refreshToken = GenerateRefreshTokenString();
        JwtFactory jwtFactory = new(_jwtOptions);
        var (Token, _ , Expires) = jwtFactory.Generate(claimsIdentity);
        Tokens tokens = new()
        {
            AccessToken = Token,
            Expires = Expires,
            RefreshToken = refreshToken,
        };

        _logger.LogTrace("Token generated");

        return tokens;
    }

    public async Task<bool> IsBlacklistedAsync(string token, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(token))
            return false;

        var key = token.ToSha512Base64UrlEncoding();
        _logger.LogTrace("Checking if token {Token} is in blacklist. Key: {Key}", token, key);
        string? cache = await _distributedCache.GetStringAsync(key, cancellationToken);
        bool exists = !string.IsNullOrEmpty(cache);
        _logger.LogDebug("Token {Token} is in blacklist: {Result}", token, exists);

        return exists;
    }

    public async Task<bool> ValidateTokenSignatureAsync(string jwt, CancellationToken cancellationToken = default)
    {
        _logger.LogTrace("Validating signature for token {JWT}", jwt);

        var validationParameters = Constants.Jwt.GetApplicationTokenValidationParameters(
            _jwtOptions.Issuer, 
            _jwtOptions.Audience);

        var tokenHandler = new JwtSecurityTokenHandler();
        TokenValidationResult result = await tokenHandler.ValidateTokenAsync(jwt, validationParameters);
        _logger.LogTrace("JWT signature is valid: {IsValid}. Error message: '{message}'", result.IsValid, result.Exception?.Message);

        return result.IsValid;
    }

    private string GenerateRefreshTokenString()
        => StringHelper.CreateSecureRandomString(40);
}

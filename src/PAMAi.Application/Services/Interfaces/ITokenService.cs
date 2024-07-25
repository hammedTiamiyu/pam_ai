using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using PAMAi.Application.Services.Models;

namespace PAMAi.Application.Services.Interfaces;
public interface ITokenService
{
    /// <summary>
    /// Blacklist or block a given token from having access.
    /// </summary>
    /// <param name="token">
    /// Token to blacklist.
    /// </param>
    /// <param name="cancellationToken">
    /// Token to cancel the operation.
    /// </param>
    /// <returns></returns>
    Task BlacklistJwtAsync(string token, CancellationToken cancellationToken = default);

    /// <summary>
    /// Generate a new access and refresh token for user.
    /// </summary>
    /// <param name="claimsIdentity">
    /// Claims of the identified user.
    /// </param>
    /// <returns>The newly-generated tokens.</returns>
    Tokens GenerateToken(ClaimsIdentity claimsIdentity);

    /// <summary>
    /// Deserialise the encoded JWT string.
    /// </summary>
    /// <param name="jwt">JWT string.</param>
    /// <returns></returns>
    JwtSecurityToken GetJwtSecurityToken(string jwt);

    /// <summary>
    /// Checks if the given token is blacklisted.
    /// </summary>
    /// <param name="token">Token</param>
    /// <param name="cancellationToken">
    /// Token to cancel the operation.
    /// </param>
    /// <returns>
    /// <see langword="true"/> if the token has been blacklisted, otherwise, <see langword="false"/>.
    /// </returns>
    Task<bool> IsBlacklistedAsync(string token, CancellationToken cancellationToken = default);

    /// <summary>
    /// Validate signature of JWT to ensure it came from this application.
    /// </summary>
    /// <param name="jwt">JSON Web Token.</param>
    /// <returns><see langword="true"/> if valid, otherwise <see langword="false"/>.</returns>
    Task<bool> ValidateTokenSignatureAsync(string jwt, CancellationToken cancellationToken = default);
}

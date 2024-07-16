using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using PAMAi.Domain.Options;

namespace PAMAi.Infrastructure.Identity;

internal sealed class JwtFactory
{
    private readonly JwtOptions _jwtOptions;

    public JwtFactory(JwtOptions jwtOptions) 
        => _jwtOptions = jwtOptions;

    /// <summary>
    /// Generate new JWT from the given claims identity.
    /// </summary>
    /// <param name="identity">
    /// User's claim identity.
    /// </param>
    /// <returns></returns>
    internal (string Token, DateTimeOffset Created, DateTimeOffset Expires) Generate(ClaimsIdentity identity)
    {
        var utcNow = DateTimeOffset.Now;
        DateTimeOffset expiresUtc = utcNow.Add(_jwtOptions.ValidFor);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(JwtRegisteredClaimNames.Iat, utcNow.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64),
        }.Union(identity.Claims);

        // Create the JWT security token and encode it.
        var jwt = new JwtSecurityToken(
            issuer: _jwtOptions.Issuer,
            audience: _jwtOptions.Audience,
            claims: claims,
            notBefore: utcNow.DateTime,
            expires: expiresUtc.DateTime,
            signingCredentials: Constants.Jwt.SigningCredentials);

        var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

        return (encodedJwt, utcNow, expiresUtc);
    }
}

using Microsoft.AspNetCore.Identity;

namespace PAMAi.Infrastructure.Identity.Models;

/// <summary>
/// Application user.
/// </summary>
public class User: IdentityUser
{
    /// <summary>
    /// User password.
    /// </summary>
    public UserPassword? UserPassword { get; set; }/* = new();*/

    /// <summary>
    /// User's refresh token.
    /// </summary>
    public List<RefreshToken> RefreshTokens { get; set; } = [];
}

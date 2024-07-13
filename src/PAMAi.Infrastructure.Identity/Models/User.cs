﻿using Microsoft.AspNetCore.Identity;

namespace PAMAi.Infrastructure.Identity.Models;

/// <summary>
/// Application user.
/// </summary>
public class User: IdentityUser
{
    /// <summary>
    /// First name.
    /// </summary>
    public string FirstName { get; set; } = string.Empty;

    /// <summary>
    /// Last name.
    /// </summary>
    public string LastName { get; set; } = string.Empty;

    /// <summary>
    /// User's refresh token.
    /// </summary>
    public List<RefreshToken> RefreshTokens { get; set; } = [];
}
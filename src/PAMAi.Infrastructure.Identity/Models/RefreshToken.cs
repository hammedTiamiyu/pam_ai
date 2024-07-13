using PAMAi.Application.Extensions;

namespace PAMAi.Infrastructure.Identity.Models;

/// <summary>
/// Refresh token.
/// </summary>
public class RefreshToken
{
    /// <summary>
    /// Refresh token.
    /// </summary>
    /// <remarks>
    /// The refresh token should be hashed using SHA-512 and Base64-URL encoded using
    /// <see cref="StringExtensions.ToSha512Base64UrlEncoding(string)"/> before being 
    /// stored in this property to be stored in the database.
    /// 
    /// <code>
    /// string token = "eyajfksjkdjaf9lkdjfa";
    /// RefreshToken t = new()
    /// {
    ///     Token = token.ToSha512Base64UrlEncoding(),
    ///     ...
    /// };
    /// </code>
    /// 
    /// <para>
    /// Comparison should be made by first hashing the user's refresh token and comparing 
    /// it against the values in the database.
    /// </para>
    /// 
    /// <code>
    /// IQueryable&lt;RefreshToken&gt; queryable = ...;
    /// RefreshToken? matchingRefreshToken = queryable
    ///     .Where(r => r.Token.ToUpper() == userRefreshToken.ToSha512Base64UrlEncoding().ToUpper())
    ///     .FirstOrDefault();
    /// </code>
    /// </remarks>
    public string Token { get; set; } = string.Empty;
    
    /// <summary>
    /// Expiration date in UTC.
    /// </summary>
    public DateTimeOffset ExpiresUtc { get; set; }

    /// <summary>
    /// Creation date in UTC.
    /// </summary>
    public DateTimeOffset CreatedUtc { get; set; }

    /// <summary>
    /// Revocation date in UTC.
    /// </summary>
    public DateTimeOffset? RevokedUtc { get; set; }

    /// <summary>
    /// Indicates if the refresh token is revoked.
    /// </summary>
    public bool IsRevoked => RevokedUtc is not null;

    /// <summary>
    /// Indicates if the refresh token is expired.
    /// </summary>
    public bool IsExpired => ExpiresUtc <= DateTimeOffset.UtcNow;

    /// <summary>
    /// Indicates if the refresh token is still active for use.
    /// </summary>
    public bool IsActive => !IsRevoked && !IsExpired;
}

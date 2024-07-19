using PAMAi.Domain.Enums;

namespace PAMAi.Application.Services.Interfaces;

/// <summary>
/// Information about the user currently logged in.
/// </summary>
/// <remarks>
/// If no user is logged in, its properties will be <see langword="null"/>.
/// </remarks>
public interface ICurrentUser
{
    /// <summary>
    /// Indicates if for the current HTTP request session, there's an identified
    /// logged-in user.
    /// </summary>
    bool Any { get; }

    /// <summary>
    /// User's ID.
    /// </summary>
    public string? UserId { get; }

    /// <summary>
    /// Role the user logged in as.
    /// </summary>
    public ApplicationRole? Role { get; }
}

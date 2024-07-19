using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using PAMAi.Application.Services.Interfaces;
using PAMAi.Domain.Enums;

namespace PAMAi.Infrastructure.Identity.Services;
internal class CurrentUser: ICurrentUser
{
    public CurrentUser(IHttpContextAccessor httpContextAccessor)
    {
        UserId = httpContextAccessor.HttpContext?.User?.Claims?.FirstOrDefault(c => string.Equals(c.Type, ClaimTypes.NameIdentifier))?.Value;
        string? roleStr = httpContextAccessor.HttpContext?.User?.Claims?.FirstOrDefault(c => string.Equals(c.Type, ClaimTypes.Role))?.Value;
        bool successful = Enum.TryParse(roleStr, out ApplicationRole role);
        if (successful)
            Role = role;
    }

    /// <inheritdoc/>
    public bool Any => UserId is not null;

    /// <inheritdoc/>
    public string? UserId { get; }

    /// <inheritdoc/>
    public ApplicationRole? Role { get; }

}

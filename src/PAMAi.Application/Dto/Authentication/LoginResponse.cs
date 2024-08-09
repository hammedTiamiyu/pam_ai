using PAMAi.Domain.Enums;

namespace PAMAi.Application.Dto.Authentication;
public record LoginResponse
{
    public string AccessToken { get; set; } = string.Empty;

    public DateTimeOffset Expires { get; set; }

    public string RefreshToken { get; set; } = string.Empty;

    /// <example>2</example>
    public ApplicationRole Role { get; set; }

    /// <example>Installer</example>
    public string RoleValue => Role.ToString();

    public bool HasAcceptedTermsOfService { get; set; }
}

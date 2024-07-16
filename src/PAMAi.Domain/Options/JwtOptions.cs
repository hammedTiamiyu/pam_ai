namespace PAMAi.Domain.Options;

public sealed class JwtOptions
{
    public static readonly string ConfigurationKey = "Authentication:Schemes:Bearer";

    public string Issuer { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
    public int ValidForInMinutes { get; set; }
    public int RefreshTokenValidForInDays { get; set; }

    public TimeSpan ValidFor => TimeSpan.FromMinutes(ValidForInMinutes);
}

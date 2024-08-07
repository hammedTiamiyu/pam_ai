using PAMAi.Domain.Entities.Base;

namespace PAMAi.Domain.Entities;

/// <summary>
/// Terms of Service which every user must accept to use PAMAi.
/// </summary>
public class TermsOfService: IEntity<int>
{
    /// <inheritdoc/>
    public int Id { get; set; }

    /// <summary>
    /// Content of the legal contract.
    /// </summary>
    public string Content { get; set; } = string.Empty;

    /// <summary>
    /// Version number.
    /// </summary>
    public double Version { get; set; }

    /// <summary>
    /// Date the legal contract was added.
    /// </summary>
    public DateTimeOffset CreatedUtc { get; set; }

    /// <summary>
    /// Date the legal contract would start being effective.
    /// </summary>
    public DateTimeOffset EffectiveFromUtc { get; set; }

    /// <summary>
    /// Date the legal contract stopped being effective.
    /// </summary>
    public DateTimeOffset? DeactivatedUtc { get; set; }

    /// <summary>
    /// Indicates whether the legal contract is still active or not.
    /// </summary>
    public bool IsActive => DateTimeOffset.UtcNow >= EffectiveFromUtc && !DeactivatedUtc.HasValue;

    /// <summary>
    /// User profiles.
    /// </summary>
    public List<UserProfile> UserProfiles { get; set; } = [];

    /// <summary>
    /// List of users and their consent to this legal contract.
    /// </summary>
    public List<UserTermsOfServiceConsent> UserTermsOfServiceConsents { get; set; } = [];
}

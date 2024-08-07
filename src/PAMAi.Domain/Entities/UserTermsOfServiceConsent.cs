using PAMAi.Domain.Entities.Base;

namespace PAMAi.Domain.Entities;

/// <summary>
/// Contains information about a user's consent to PAMAi's legal
/// terms of service.
/// </summary>
public class UserTermsOfServiceConsent: IEntity
{
    /// <summary>
    /// Id of the profile of the user who accepted the agreement.
    /// </summary>
    public Guid UserProfileId { get; set; }

    /// <summary>
    /// Legal terms of service's Id.
    /// </summary>
    public int TermsOfServiceId { get; set; }

    /// <summary>
    /// The date the user accepted the legal contract.
    /// </summary>
    public DateTimeOffset? AcceptedDateUtc { get; set; }

    /// <summary>
    /// Indicates if the user has accepted the legal contract.
    /// </summary>
    public bool IsAccepted => AcceptedDateUtc is not null;

    /// <summary>
    /// User's profile.
    /// </summary>
    public UserProfile? UserProfile { get; set; }

    /// <summary>
    /// Legal terms of service.
    /// </summary>
    public TermsOfService? TermsOfService { get; set; }
}

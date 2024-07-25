using PAMAi.Domain.Entities.Base;
using PAMAi.Domain.Enums;

namespace PAMAi.Domain.Entities;

/// <summary>
/// Profile of user in the application.
/// </summary>
public sealed class UserProfile: IEntity<Guid>
{
    /// <inheritdoc/>
    public Guid Id { get; set; }

    /// <summary>
    /// First name.
    /// </summary>
    public string FirstName { get; set; } = string.Empty;

    /// <summary>
    /// Last name.
    /// </summary>
    public string LastName { get; set; } = string.Empty;

    /// <summary>
    /// Full name.
    /// </summary>
    public string FullName => $"{FirstName} {LastName}";

    /// <summary>
    /// Bio.
    /// </summary>
    public string Bio { get; set; } = string.Empty;

    /// <summary>
    /// User's gender.
    /// </summary>
    public Gender? Gender { get; set; }

    /// <summary>
    /// House number.
    /// </summary>
    public string? HouseNumber { get; set; }

    /// <summary>
    /// Street.
    /// </summary>
    public string? Street { get; set; }

    /// <summary>
    /// City.
    /// </summary>
    public string? City { get; set; }

    /// <summary>
    /// Residence state primary key.
    /// </summary>
    public long? StateId { get; set; }

    /// <summary>
    /// Company.
    /// </summary>
    public string? CompanyName { get; set; }

    /// <summary>
    /// Identity user ID.
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// Residence state.
    /// </summary>
    public State? State { get; set; }
}

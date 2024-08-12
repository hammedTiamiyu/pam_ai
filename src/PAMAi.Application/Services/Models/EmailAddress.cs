namespace PAMAi.Application.Services.Models;

/// <summary>
/// Email address details.
/// </summary>
public record EmailAddress
{
    /// <summary>
    /// Name of the mailbox owner.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Email address of the mailbox.
    /// </summary>
    public required string Address { get; set; }
}

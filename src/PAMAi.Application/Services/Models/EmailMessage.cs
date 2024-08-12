namespace PAMAi.Application.Services.Models;

public record EmailMessage
{
    /// <summary>
    /// Message subject.
    /// </summary>
    public required string Subject { get; set; }

    /// <summary>
    /// Message body.
    /// </summary>
    /// <remarks>
    /// Can be plain text or HTML.
    /// </remarks>
    public required string Body { get; set; }

    /// <summary>
    /// Format of the message body.
    /// </summary>
    public required EmailBodyFormat Format { get; set; }

    /// <summary>
    /// Sender of the mail message.
    /// </summary>
    /// <remarks>
    /// Set as <see langword="null"/> to use the application's default
    /// sender details.
    /// </remarks>
    public EmailAddress? From { get; set; }

    /// <summary>
    /// To.
    /// </summary>
    public List<EmailAddress> To { get; set; } = [];

    /// <summary>
    /// Blind carbon copy (BCC).
    /// </summary>
    public List<EmailAddress> Bcc { get; set; } = [];

    /// <summary>
    /// Carbon copy (CC).
    /// </summary>
    public List<EmailAddress> Cc { get; set; } = [];
}

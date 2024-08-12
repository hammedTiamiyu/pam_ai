using System.Diagnostics.CodeAnalysis;

namespace PAMAi.Application.Services.Models;

/// <summary>
/// Contents to be delivered through one or more notification channels.
/// </summary>
public record NotificationContents
{
    /// <inheritdoc cref="EmailContent"/>
    public EmailContent? Email { get; set; }

    /// <inheritdoc cref="PushContent"/>
    public PushContent? Push { get; set; }

    /// <inheritdoc cref="SmsContent"/>
    public SmsContent? Sms { get; set; }

    /// <summary>
    /// User ID of the recipient.
    /// </summary>
    public required string RecipientUserId { get; set; }

    /// <summary>
    /// Content to be delivered through email.
    /// </summary>
    public record EmailContent
    {
        /// <summary>
        /// Email subject.
        /// </summary>
        public required string Subject { get; set; }

        /// <summary>
        /// Email body.
        /// </summary>
        public required string Body { get; set; }

        /// <summary>
        /// Format of the email body.
        /// </summary>
        public required EmailBodyFormat Format { get; set; }
    }

    /// <summary>
    /// Push notification content.
    /// </summary>
    public record PushContent
    {
        public PushContent()
        {
        }

        /// <summary>
        /// Creates a new instance of <see cref="PushContent"/>.
        /// </summary>
        /// <param name="title">
        /// Notification title.
        /// </param>
        /// <param name="body">
        /// Notification body.
        /// </param>
        [SetsRequiredMembers]
        public PushContent(string title, string body)
        {
            Title = title;
            Body = body;
        }

        /// <summary>
        /// Notification title.
        /// </summary>
        public required string Title { get; set; }

        /// <summary>
        /// Notification body.
        /// </summary>
        public required string Body { get; set; }
    }

    /// <summary>
    /// SMS notification content.
    /// </summary>
    public record SmsContent
    {
        public SmsContent()
        {
        }

        /// <summary>
        /// Creates a new instance of <see cref="SmsContent"/>.
        /// </summary>
        /// <param name="message">
        /// SMS message.
        /// </param>
        [SetsRequiredMembers]
        public SmsContent(string message)
        {
            Message = message;
        }

        /// <summary>
        /// Message body
        /// </summary>
        public required string Message { get; set; }
    }
}

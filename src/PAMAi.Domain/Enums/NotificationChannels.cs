namespace PAMAi.Domain.Enums;

/// <summary>
/// Notification channels used in the application.
/// </summary>
[Flags]
public enum NotificationChannels
{
    /// <summary>
    /// No notification channel.
    /// </summary>
    None = 0,

    /// <summary>
    /// SMS notification.
    /// </summary>
    Sms = 1,

    /// <summary>
    /// Email notification.
    /// </summary>
    Email = 2,

    /// <summary>
    /// Mobile push notification.
    /// </summary>
    Push = 4,
}

namespace PAMAi.Application.Services.Models.Internal;

/// <summary>
/// Notification details and preferences of the recipient
/// user.
/// </summary>
internal class UserNotificationDetails
{
    /// <summary>
    /// Recipeint's full name.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Recipient's email address.
    /// </summary>
    internal string Email { get; set; } = string.Empty;

    /// <summary>
    /// Recipient's phone number.
    /// </summary>
    internal string PhoneNumber { get; set; } = string.Empty;

    /// <summary>
    /// Recipient's device token for push notifications.
    /// </summary>
    internal string DeviceToken { get; set; } = string.Empty;
}

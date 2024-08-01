using PAMAi.Application.Services.Models;

namespace PAMAi.Application.Services.Interfaces;

/// <summary>
/// Service responsible for sending push notification to the user's
/// mobile devices.
/// </summary>
public interface IPushNotificationService
{
    /// <summary>
    /// Send push notification.
    /// </summary>
    /// <param name="content">Notification content.</param>
    /// <param name="deviceToken">Device token of the recipient.</param>
    /// <param name="cancellationToken">
    /// Token for cancelling the operation.
    /// </param>
    /// <returns>
    /// The result of the operation.
    /// </returns>
    Task<Result> SendAsync(NotificationContents.PushContent content, string deviceToken, CancellationToken cancellationToken = default);

    /// <summary>
    /// Send push notification.
    /// </summary>
    /// <param name="title">
    /// Notification title.
    /// </param>
    /// <param name="body">
    /// Notification body.
    /// </param>
    /// <param name="deviceToken">
    /// Device token of the recipient.
    /// </param>
    /// <param name="cancellationToken">
    /// Token for cancelling the operation.
    /// </param>
    /// <returns>
    /// The result of the operation.
    /// </returns>
    Task<Result<string>> SendAsync(string title, string body, string deviceToken, CancellationToken cancellationToken = default);
}

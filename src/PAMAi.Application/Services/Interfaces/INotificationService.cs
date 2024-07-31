using PAMAi.Application.Dto.SMS;
using PAMAi.Application.Services.Models;
using PAMAi.Domain.Enums;

namespace PAMAi.Application.Services.Interfaces;

public interface INotificationService
{
    Task<Result<string>> SendPushNotificationAsync(string title, string body, string token);
    Task<Result<SmsResponse>> SendCredentialsAsync(string phoneNumbers, string resetPasswordLink);
    Task<Result<SmsResponse>> TestSMSAsync(string phoneNumber, string sms);
    Task<Result<SmsResponse>> TestEmailAsync(string email, string body);

    /// <summary>
    /// Send notification.
    /// </summary>
    /// <param name="contents">
    /// Contents of the notification to be sent to the different channels.
    /// </param>
    /// <param name="channels">
    /// Notification channel(s) to use.
    /// </param>
    /// <param name="cancellationToken">
    /// Token for cancelling the operation.
    /// </param>
    /// <returns>
    /// One of the following:
    /// <list type="bullet">
    ///   <item>
    ///     A successful result if one or more of the notifications sent through the indicated channels worked.
    ///   </item>
    ///   <item>
    ///     A failed result if all notification channels failed.
    ///   </item>
    /// </list>
    /// </returns>
    Task<Result> SendAsync(NotificationContents contents, NotificationChannels channels, CancellationToken cancellationToken = default);
}
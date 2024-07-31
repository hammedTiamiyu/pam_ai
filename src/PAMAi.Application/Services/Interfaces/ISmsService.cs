using PAMAi.Application.Dto.SMS;
using PAMAi.Application.Services.Models;

namespace PAMAi.Application.Services.Interfaces;
public interface ISmsService
{
    /// <summary>
    /// Send SMS message.
    /// </summary>
    /// <param name="content">
    /// SMS details.
    /// </param>
    /// <param name="phoneNumber">
    /// Recipient's phone number.
    /// </param>
    /// <param name="cancellationToken">
    /// Token to cancel the operation.
    /// </param>
    /// <returns>
    /// The result of the operation.
    /// </returns>
    Task<Result> SendMessageAsync(
        NotificationContents.SmsContent content,
        string phoneNumber,
        CancellationToken cancellationToken = default);

    Task<Result<SmsResponse>> SendSmsAsync(SmsRequest message, CancellationToken cancellationToken = default);
}

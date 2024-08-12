using PAMAi.Application.Services.Models;

namespace PAMAi.Application.Services.Interfaces;

/// <summary>
/// Service for email operations.
/// </summary>
public interface IEmailService
{
    /// <summary>
    /// Send email notification.
    /// </summary>
    /// <param name="content">
    /// Email content.
    /// </param>
    /// <param name="toAddress">
    /// Email address of the recipient.
    /// </param>
    /// <param name="toName">
    /// Full name of the recipient.
    /// </param>
    /// <param name="cancellationToken">
    /// Token for cancelling the operation.
    /// </param>
    /// <returns>
    /// The result of the operation.
    /// </returns>
    Task<Result> SendAsync(NotificationContents.EmailContent content, string toAddress, string toName = "", CancellationToken cancellationToken = default);

    /// <summary>
    /// Send email.
    /// </summary>
    /// <param name="emailMessage">
    /// Email message.
    /// </param>
    /// <param name="cancellationToken">
    /// Token for cancelling the operation.
    /// </param>
    /// <returns>
    /// The result of the operation.
    /// </returns>
    Task<Result> SendAsync(EmailMessage emailMessage, CancellationToken cancellationToken = default);
}

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
    /// <param name="to">
    /// Email recipient.
    /// </param>
    /// <param name="cancellationToken">
    /// Token for cancelling the operation.
    /// </param>
    /// <returns>
    /// The result of the operation.
    /// </returns>
    Task<Result> SendAsync(NotificationContents.EmailContent content, string to, CancellationToken cancellationToken = default);
}

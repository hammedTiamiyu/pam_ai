using Microsoft.Extensions.Logging;
using PAMAi.Application;
using PAMAi.Application.Services.Interfaces;
using PAMAi.Application.Services.Models;

namespace PAMAi.Infrastructure.ExternalServices.Services.Email;

internal class EmailService: IEmailService
{
    private readonly ILogger<EmailService> _logger;

    public EmailService(ILogger<EmailService> logger)
    {
        _logger = logger;
    }

    public Task<Result> SendAsync(NotificationContents.EmailContent content, string to, CancellationToken cancellationToken = default)
    {
        _logger.LogWarning("Cannot send email. This feature is yet to be implemented");
        return Task.FromResult(Result.Failure(new Error("Not implemented")));
    }
}

using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using MimeKit.Text;
using PAMAi.Application;
using PAMAi.Application.Services.Interfaces;
using PAMAi.Application.Services.Models;
using PAMAi.Domain.Options;
using PAMAi.Infrastructure.ExternalServices.Errors;

namespace PAMAi.Infrastructure.ExternalServices.Services.Email;

internal class EmailService: IEmailService
{
    private readonly ILogger<EmailService> _logger;
    private readonly EmailOptions _emailOptions;

    public EmailService(ILogger<EmailService> logger, IOptions<EmailOptions> emailOptions)
    {
        _logger = logger;
        _emailOptions = emailOptions.Value;
    }

    public async Task<Result> SendAsync(
        NotificationContents.EmailContent content,
        string toAddress,
        string toName = "",
        CancellationToken cancellationToken = default)
    {
        EmailMessage emailMessage = ToEmailAddress(content, toAddress, toName);
        return await SendAsync(emailMessage, cancellationToken);
    }

    public async Task<Result> SendAsync(EmailMessage emailMessage, CancellationToken cancellationToken = default)
    {
        MimeMessage message = BuildMailKitMessage(emailMessage);

        try
        {
            using var client = new SmtpClient();
            await client.ConnectAsync(_emailOptions.Smtp.Host, _emailOptions.Smtp.Port, SecureSocketOptions.Auto, cancellationToken);
            _logger.LogTrace("Connected to SMTP server");

            await client.AuthenticateAsync(_emailOptions.Smtp.UserName, _emailOptions.Smtp.Password, cancellationToken);
            _logger.LogTrace("SMTP user authenticated");

            string response = await client.SendAsync(message, cancellationToken);
            _logger.LogDebug("Message sent. SMTP server response: '{Response}'", response);

            await client.DisconnectAsync(true, cancellationToken);
            _logger.LogTrace("Disconnected from SMTP server");

            return Result.Success();
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "An error occurred while sending the email. Message: {Message}", exception.Message);
            return Result.Failure(EmailServiceErrors.MailNotSent);
        }
    }

    private static TextFormat GetTextPart(EmailBodyFormat bodyFormat)
    {
        return bodyFormat switch
        {
            EmailBodyFormat.Text => TextFormat.Text,
            EmailBodyFormat.Html => TextFormat.Html,
            _ => throw new NotImplementedException(),
        };
    }

    private static EmailMessage ToEmailAddress(NotificationContents.EmailContent emailContent, string toAddress, string toName)
    {
        return new EmailMessage
        {
            Body = emailContent.Body,
            Format = emailContent.Format,
            Subject = emailContent.Subject,
            To = [new EmailAddress { Address = toAddress, Name = toName }],
        };
    }

    private static MailboxAddress ToMailBoxAddress(EmailAddress emailAddress)
    {
        return new MailboxAddress(emailAddress.Name, emailAddress.Address);
    }

    private MimeMessage BuildMailKitMessage(EmailMessage message)
    {
        if (message.Cc.Count == 0 &&
            message.To.Count == 0 &&
            message.Bcc.Count == 0)
        {
            throw new ArgumentException("The email message must have at least one recipient (To, CC, or BCC).", nameof(message));
        }

        var result = new MimeMessage()
        {
            Subject = message.Subject,
            Body = new TextPart(GetTextPart(message.Format))
            {
                Text = message.Body,
            },
        };

        if (message.From is null)
            result.From.Add(new MailboxAddress(_emailOptions.Sender.Name, _emailOptions.Sender.Address));
        else
            result.From.Add(ToMailBoxAddress(message.From));

        foreach (EmailAddress address in message.To)
        {
            result.To.Add(ToMailBoxAddress(address));
        }
        foreach (EmailAddress address in message.Cc)
        {
            result.Cc.Add(ToMailBoxAddress(address));
        }
        foreach (EmailAddress address in message.Bcc)
        {
            result.Bcc.Add(ToMailBoxAddress(address));
        }

        return result;
    }
}

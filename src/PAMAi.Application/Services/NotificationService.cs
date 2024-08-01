using Microsoft.Extensions.Logging;
using PAMAi.Application.Dto.SMS;
using PAMAi.Application.Errors;
using PAMAi.Application.Exceptions;
using PAMAi.Application.Services.Interfaces;
using PAMAi.Application.Services.Models;
using PAMAi.Application.Services.Models.Internal;
using PAMAi.Domain.Enums;

namespace PAMAi.Application.Services;

internal sealed class NotificationService: INotificationService
{
    private readonly IAccountService _accountService;
    private readonly IEmailService _emailService;
    private readonly ILogger<NotificationService> _logger;
    private readonly IPushNotificationService _pushNotificationService;
    private readonly ISmsService _smsService;

    public NotificationService(
        IAccountService accountService,
        IEmailService emailService,
        ILogger<NotificationService> logger,
        IPushNotificationService pushNotificationService,
        ISmsService smsService)
    {
        _accountService = accountService;
        _emailService = emailService;
        _logger = logger;
        _pushNotificationService = pushNotificationService;
        _smsService = smsService;

    }

    public async Task<Result> SendAsync(
        NotificationContents contents,
        NotificationChannels channels,
        CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Sending notifications to user {Id}. Channel(s): {Channels}",
            contents.RecipientUserId,
            channels);

        if (channels == NotificationChannels.None)
        {
            _logger.LogInformation("No notification channel was selected. Notifications not sent");
            return Result.Success();
        }

        List<Task<Result>> notificationTasks = [];
        UserNotificationDetails recipientNotificationDetails = await GetUserNotificationDetailsAsync(
            contents.RecipientUserId,
            cancellationToken);

        if (channels.HasChannel(NotificationChannels.Sms))
        {
            _logger.LogTrace("Sending notification through the SMS channel");
            notificationTasks.Add(_smsService.SendMessageAsync(contents.Sms,
                                                               recipientNotificationDetails.PhoneNumber,
                                                               cancellationToken));
        }
        if (channels.HasChannel(NotificationChannels.Email))
        {
            _logger.LogTrace("Sending notification through the email channel");
            notificationTasks.Add(_emailService.SendAsync(contents.Email,
                                                          recipientNotificationDetails.Email,
                                                          cancellationToken));
        }
        if (channels.HasChannel(NotificationChannels.Push))
        {
            _logger.LogTrace("Sending notification through the push notification channel");
            notificationTasks.Add(_pushNotificationService.SendAsync(contents.Push,
                                                                     recipientNotificationDetails.DeviceToken,
                                                                     cancellationToken));
        }

        Result[] results = await Task.WhenAll(notificationTasks);
        bool allFailed = results.All(r => r.IsFailure);
        if (allFailed)
        {
            _logger.LogError("All requested notification channels failed to send the notification. Check the logs for more details. Channel(s): {Channels}",
                channels);
            return Result.Failure(NotificationErrors.SendFailed);
        }

        bool anyNotificationFailed = results.Any(r => r.IsFailure);
        if (anyNotificationFailed)
            _logger.LogWarning("One or more notifications failed. Check the logs for more information");

        return Result.Success();
    }

    /// <summary>
    /// Sends Password After USer accts and assets profile have been created
    /// </summary>
    /// <param name="phoneNumbers"></param>
    /// <param name="resetPasswordLink"></param>
    /// <returns></returns>
    public async Task<Result<SmsResponse>> SendCredentialsAsync(string phoneNumbers, string resetPasswordLink)
    {
        //TODO : fetch user's Username and password and then send it as part of the message body
        var message = new SmsRequest
        {
            To = phoneNumbers,
            Sms = $"Reset your password using this link: {resetPasswordLink}",

        };

        return await _smsService.SendSmsAsync(message);
    }

    /// <summary>
    /// Test SMS  with termii, always cofirm APIKEY and SENDER ID
    /// </summary>
    /// <param name="phoneNumber"></param>
    /// <param name="sms"></param>
    /// <returns></returns>
    public async Task<Result<SmsResponse>> TestSMSAsync(string phoneNumber, string sms)
    {

        var message = new SmsRequest
        {
            To = phoneNumber,
            Sms = $"test SMS From PAM AI {sms}",

        };

        return await _smsService.SendSmsAsync(message);
    }

    public async Task<Result<SmsResponse>> TestEmailAsync(string email, string body)
    {
        //TODO TEST EMAIL NOTIFS
        var message = new SmsRequest
        {
            To = email,
            Sms = $"test SMS From PAM AI ",

        };

        return await _smsService.SendSmsAsync(message);
    }

    /// <summary>
    /// Send Push notifications using Google FCM
    /// </summary>
    /// <param name="title"></param>
    /// <param name="body"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    public async Task<Result<string>> SendPushNotificationAsync(string title, string body, string token)
    {

        return await _pushNotificationService.SendAsync(title, body, token);
    }

    private async Task<UserNotificationDetails> GetUserNotificationDetailsAsync(string userId, CancellationToken cancellationToken = default)
    {
        var profile = await _accountService.GetProfileAsync(userId, cancellationToken);

        return new UserNotificationDetails
        {
            Email = profile.Data?.Email ?? string.Empty,
            PhoneNumber = profile.Data?.PhoneNumber ?? string.Empty,
            // HACK: Device token hasn't been implemented yet.
            DeviceToken = string.Empty,
        };
    }
}


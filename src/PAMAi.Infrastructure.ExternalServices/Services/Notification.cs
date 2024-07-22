using PAMAi.Application.Dto.SMS;
using PAMAi.Application.Services.Interfaces;
using PAMAi.Application;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using PAMAi.Domain.Options;
using Microsoft.Extensions.Options;
using PAMAi.Infrastructure.ExternalServices.Services.SMS;
using PAM_Ai.ExternalServices.Services.FcmNotifications;

namespace PAMAi.Infrastructure.ExternalServices.Services;

public class NotificationService : INotificationService
{
    private readonly ISmsRepository _smsRepository;
    private readonly TermiiOptions _settings;
    private readonly ILogger<SmsRepository> _logger;
    private readonly FcmService _fcmService;

    public NotificationService(ISmsRepository smsRepository, ILogger<SmsRepository> logger,
        IOptions<TermiiOptions> settings, FcmService fcmService)
    {
        _smsRepository = smsRepository;
        _logger = logger;
        _settings = settings.Value;
        _fcmService = fcmService;

    }

    public async Task<Result<SmsResponse>> SendCredentialsAsync(string phoneNumbers, string resetPasswordLink)
    {
        //TODO : fetch user's Username and password and then send it as part of the message body
        var message = new SmsRequest
        {
            To = phoneNumbers,
            Sms = $"Reset your password using this link: {resetPasswordLink}",

        };

        return await _smsRepository.SendSmsAsync(message);
    }

    public async Task<Result<SmsResponse>> TestSMSAsync(string phoneNumber, string sms)
    {

        var message = new SmsRequest
        {
            To = phoneNumber,
            Sms = $"test SMS From PAM AI {sms}",

        };

        return await _smsRepository.SendSmsAsync(message);
    }

    public async Task<Result<SmsResponse>> TestEmailAsync(string email, string body)
    {
        //TODO TEST EMAIL NOTIFS
        var message = new SmsRequest
        {
            To = email,
            Sms = $"test SMS From PAM AI ",

        };

        return await _smsRepository.SendSmsAsync(message);
    }

    public Task<Result<string>> SendPushNotificationAsync(string title, string body, string token)
    {
        //throw new NotImplementedException();
        return await _fcmService.SendNotificationAsync(title, body, token);
    }
}


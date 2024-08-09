using FirebaseAdmin;
using FirebaseAdmin.Messaging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using PAMAi.Application;
using PAMAi.Application.Services.Interfaces;
using PAMAi.Application.Services.Models;
using PAMAi.Infrastructure.ExternalServices.Errors;

namespace PAMAi.Infrastructure.ExternalServices.Services.PushNotification;

internal sealed class PushNotificationService: IPushNotificationService
{
    private readonly FirebaseApp _firebaseApp;
    private readonly ILogger<PushNotificationService> _logger;

    public PushNotificationService(IConfiguration configuration, ILogger<PushNotificationService> logger)
    {
        _logger = logger;

        //var credentialsPath = configuration["Firebase:CredentialsPath"];
        //_firebaseApp = FirebaseApp.Create(new AppOptions
        //{
        //    Credential = GoogleCredential.FromFile(credentialsPath)
        //});

        //_logger.LogInformation("Firebase App initialized with credentials from {path}", credentialsPath);
    }

    public async Task<Result> SendAsync(NotificationContents.PushContent content, string deviceToken, CancellationToken cancellationToken = default)
    {
        var result = await SendAsync(content.Title, content.Body, deviceToken, cancellationToken);
        return Result.From(result);
    }

    public async Task<Result<string>> SendAsync(string title, string body, string token, CancellationToken cancellationToken = default)
    {
        var message = new Message
        {
            Notification = new Notification
            {
                Title = title,
                Body = body
            },
            Token = token
        };

        try
        {
            string response = await FirebaseMessaging.DefaultInstance.SendAsync(message, cancellationToken);
            _logger.LogInformation("Notification sent successfully: {response}", response);
            return Result<string>.Success(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Could not send push notification. Message: {Message}", ex.Message);
            return Result<string>.Failure(FcmErrors.FcmException with
            {
                Description = "Failed to Notifications",
            });
        }
    }
}

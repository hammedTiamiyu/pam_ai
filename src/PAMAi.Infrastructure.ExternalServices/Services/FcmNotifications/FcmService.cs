using FirebaseAdmin;
using FirebaseAdmin.Messaging;
using Google.Apis.Auth.OAuth2;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using PAMAi.Application;
using PAMAi.Infrastructure.ExternalServices.Errors;

namespace PAM_Ai.ExternalServices.Services.FcmNotifications;
public class FcmService
{
    private readonly FirebaseApp _firebaseApp;
    private readonly ILogger<FcmService> _logger;
    private IConfiguration _configuration;

    public FcmService(ILogger<FcmService> logger, IConfiguration configuration)
    {
        var credentialsPath = configuration["Firebase:CredentialsPath"];
        _logger = logger;

        _firebaseApp = FirebaseApp.Create(new AppOptions
        {
            Credential = GoogleCredential.FromFile(credentialsPath)
        });

        _logger.LogInformation("Firebase App initialized with credentials from {path}", credentialsPath);
    }

    public FcmService(IConfiguration configuration, ILogger<FcmService> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<Result<string>> SendNotificationAsync(string title, string body, string token)
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
            string response = await FirebaseMessaging.DefaultInstance.SendAsync(message);
            _logger.LogInformation("Notification sent successfully: {response}", response);
            return Result<string>.Success(response);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Exception: {ex.Message}");
            return Result<string>.Failure(FcmErrors.FcmException with
            {
                Description = "Failed to Notifications",
            });
        }
    }
}

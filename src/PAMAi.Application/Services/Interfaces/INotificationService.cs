using PAMAi.Application.Dto.SMS;

namespace PAMAi.Application.Services.Interfaces;
public interface INotificationService
{
    Task<Result<string>> SendPushNotificationAsync(string title, string body, string token);
    Task<Result<SmsResponse>> SendCredentialsAsync(string phoneNumbers, string resetPasswordLink);
    Task<Result<SmsResponse>> TestSMSAsync(string phoneNumber, string sms);
    Task<Result<SmsResponse>> TestEmailAsync(string email, string body);
}
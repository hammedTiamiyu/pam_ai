using PAMAi.Application.Dto.SMS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PAMAi.Application.Services.Interfaces;
public interface INotificationService
{
    Task<Result<string>> SendPushNotificationAsync(string title, string body, string token);
    Task<Result<SmsResponse>> SendCredentialsAsync(string phoneNumbers, string resetPasswordLink);
    Task<Result<SmsResponse>> TestSMSAsync(string phoneNumber, string sms);
    Task<Result<SmsResponse>> TestEmailAsync(string email, string body);
}
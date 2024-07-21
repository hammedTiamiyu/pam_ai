using Microsoft.AspNetCore.Mvc;
using PAM_Ai.ExternalServices.Services;
using PAMAi.Application.Dto.Account;
using PAMAi.Application.Dto.SMS;

namespace PAMAi.API.Controllers;

/// <summary>
/// Notifications for SMS and email
/// </summary>
public class NotificationsController : ControllerBase
{
    private readonly NotificationService _notificationService;

    public NotificationsController(NotificationService notificationService)
    {
        _notificationService = notificationService;
    }


    /// <summary>
    /// sends SMS credential to user so they can reset passwords
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost("password-reset")]
    public async Task<IActionResult> SendPasswordResetSms([FromBody] PasswordResetRequest request)
    {
        var result = await _notificationService.SendCredentialsAsync(request.PhoneNumber, request.ResetLink);
        return result.IsSuccess ? Ok(result) : BadRequest(result.Error);
    }

    /// <summary>
    /// Tests Termii SMS API if its functional
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost("test-sms")]
    public async Task<IActionResult> TestSmsNotification([FromBody] SmsRequest request)
    {
        var result = await _notificationService.TestSMSAsync(request.To, request.Sms);
        return result.IsSuccess ? Ok(result) : BadRequest(result.Error);
    }
}

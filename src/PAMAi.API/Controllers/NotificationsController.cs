using PAMAi.Application.Dto.Account;
using PAMAi.Application.Dto.FcmPushNotifications;
using PAMAi.Application.Dto.SMS;
using PAMAi.Application.Services.Interfaces;
using PAMAi.Infrastructure.ExternalServices.Services;

namespace PAMAi.API.Controllers;

/// <summary>
/// Notifications for SMS and email
/// </summary>
public class NotificationsController: BaseController
{
    private readonly INotificationService _notificationService;
    private readonly IActionResult _genericFailedLoginResponse;

    public NotificationsController(INotificationService notificationService, 
        IHttpContextAccessor httpContextAccessor) : base (httpContextAccessor)
    {
        _notificationService = notificationService;
    }


    /// <summary>
    /// sends SMS credential to user so they can reset passwords
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost("password-reset")]
    [ProducesResponseType(typeof(SmsResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
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
    [HttpGet("test-sms")]
    [ProducesResponseType(typeof(SmsResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    //public async Task<IActionResult> TestSmsNotification([FromBody] SmsRequest request)
    public async Task<IActionResult> TestSmsNotification()
    {
        //var result = await _notificationService.TestSMSAsync(request.To, request.Sms);
        var result = await _notificationService.TestSMSAsync("2347032519605", "TEST SMS");
        //return result.IsSuccess ? Ok(result) : BadRequest(result.Error);
        return result.Match(
            onSuccess: () => Ok(result),
            onFailure: ErrorResult);
    }

    /// <summary>
    /// Test Push notifications, if its functional before injecting into services
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost("test-push-notification")]
    [ProducesResponseType(typeof(SmsResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> SendNotification([FromBody] PushNotificationRequest request)
    {
        var result = await _notificationService.SendPushNotificationAsync(request.Title, request.Body, request.Token);
        if (result.IsSuccess)
        {
            return Ok(result);
        }
        return BadRequest(result.Error);
    }
}

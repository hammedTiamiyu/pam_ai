using System.Diagnostics;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using PAM_Ai.PAMAi.Infrastructure.ExternalServices.Errors;
using PAMAi.Application;
using PAMAi.Application.Dto.SMS;
using PAMAi.Application.Services.Interfaces;
using PAMAi.Application.Services.Models;
using PAMAi.Domain.Options;
using PAMAi.Infrastructure.ExternalServices.Errors;
using PAMAi.Infrastructure.ExternalServices.Models;
using PAMAi.Infrastructure.ExternalServices.Validation;
using RestSharp;

namespace PAMAi.Infrastructure.ExternalServices.Services.SMS
{
    internal sealed class SmsService: ISmsService
    {
        private readonly TermiiOptions _settings;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<SmsService> _logger;

        public SmsService(IHttpClientFactory httpClientFactory, ILogger<SmsService> logger, IOptions<TermiiOptions> settings)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
            _settings = settings.Value;
        }

        public async Task<Result> SendMessageAsync(NotificationContents.SmsContent content, string phoneNumber, CancellationToken cancellationToken = default)
        {
            SmsRequest request = new()
            {
                Sms = content.Message,
                To = phoneNumber,
            };
            var result = await SendSmsAsync(request, cancellationToken);

            return Result.From(result);
        }

        public async Task<Result<SmsResponse>> SendSmsAsync(SmsRequest message, CancellationToken cancellationToken = default)
        {
            if (!PhoneNumberValidator.AreValid(message.To))
            {
                _logger.LogError("Cannot send SMS. Phone number {Number} is invalid", message.To);
                return Result<SmsResponse>.Failure(SMSErrors.PhoneNumberValidation with
                {
                    Description = "Numbers must start with '234' and be 13 digits long.",
                });
            }

            var client = new RestClient(_settings.BaseUrl);
            var request = new RestRequest("api/sms/send", Method.Post);
            request.AddHeader("Content-Type", "application/json");

            TermiiMessagePayload payload = new()
            {
                To = message.To,
                Sms = message.Sms,
                From = _settings.From,
                Type = _settings.Type,
                ApiKey = _settings.Apikey,
                Channel = _settings.Channel,
            };
            request.AddJsonBody(payload);

            try
            {
                var watch = Stopwatch.StartNew();
                var response = await client.ExecuteAsync(request, cancellationToken);
                watch.Stop();
                _logger.LogDebug("POST {Url} returned status code {Code} in {Time} ms",
                    response.ResponseUri?.ToString(),
                    (int)response.StatusCode,
                    watch.ElapsedMilliseconds);

                if (response.IsSuccessful)
                {
                    var smsResponse = JsonConvert.DeserializeObject<SmsResponse>(response.Content!);
                    _logger.LogInformation("Successsfully sent SMS to '{PhoneNumber}'. API response: {@Response}",
                        message.To,
                        smsResponse);
                    return Result<SmsResponse>.Success(smsResponse!);
                }
                else
                {
                    _logger.LogError("Failed to send SMS to '{PhoneNumber}'. API response: '{@Response}'",
                        message.To,
                        response.Content);
                    return Result<SmsResponse>.Failure(SMSErrors.SMSFailure with
                    {
                        Description = "Wasn't Successful, Check Params",
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception: {Message}", ex.Message);
                return Result<SmsResponse>.Failure(SMSErrors.SMSException with
                {
                    Description = "SMSException from Termii",
                });
            }
        }
    }
}

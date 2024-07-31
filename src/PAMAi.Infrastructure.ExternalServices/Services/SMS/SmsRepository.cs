using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using PAM_Ai.PAMAi.Infrastructure.ExternalServices.Errors;
using PAMAi.Application;
using PAMAi.Application.Dto.SMS;
using PAMAi.Application.Services.Interfaces;
using PAMAi.Domain.Options;
using PAMAi.Infrastructure.ExternalServices.Validation;
using RestSharp;

namespace PAMAi.Infrastructure.ExternalServices.Services.SMS
{
    public class SmsRepository: ISmsRepository
    {
        private readonly string _baseUrl;
        private readonly string _apiKey;
        private readonly TermiiOptions _settings;
        private readonly ILogger<SmsRepository> _logger;
        private IConfiguration _configuration;

        public SmsRepository(IConfiguration configuration, ILogger<SmsRepository> logger, IOptions<TermiiOptions> settings)
        {

            _logger = logger;
            _settings = settings.Value;
            _configuration = configuration;

        }

        public async Task<Result<SmsResponse>> SendSmsAsync(SmsRequest message)
        {
            if (!PhoneNumberValidator.AreValid(message.To))
            {
                return Result<SmsResponse>.Failure(SMSErrors.PhoneNumberValidation with
                {
                    Description = "Numbers must start with '234' and be 13 digits long.",
                });
            }

            
            var client = new RestClient(_settings.BaseUrl);
            var request = new RestRequest("api/sms/send", Method.Post);
            request.AddHeader("Content-Type", "application/json");

            var body = new
            {
                to = message.To,
                from = _settings.From,
                sms = message.Sms,
                type = _settings.Type,
                api_key = _settings.Apikey,
                channel = _settings.Channel,
                //media = new
                //{
                //    url = message.MediaUrl,
                //    caption = message.MediaCaption
                //}
            };

            request.AddJsonBody(body);

            try
            {
                var response = await client.ExecuteAsync(request);
                if (response.IsSuccessful)
                {
                    var smsResponse = JsonConvert.DeserializeObject<SmsResponse>(response.Content);
                    _logger.LogError("Successsfully Sent SMS to '{ phonenumber }'", message.To);
                    return Result<SmsResponse>.Success(smsResponse);
                }
                else
                {
                    _logger.LogError("Failed to Send SMS to '{ phonenumber }'", message.To);
                    return Result<SmsResponse>.Failure(SMSErrors.SMSFailure with
                    {
                        Description = "Wasn't Successful, Check Params",
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception: {ex.Message}");
                return Result<SmsResponse>.Failure(SMSErrors.SMSException with
                {
                    Description = "SMSException from Termii",
                });
            }
        }

    }
}

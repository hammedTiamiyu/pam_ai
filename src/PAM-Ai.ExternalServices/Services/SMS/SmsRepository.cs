﻿using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using PAM_Ai.ExternalServices.Errors;
using PAM_Ai.ExternalServices.Validation;
using PAMAi.Application;
using PAMAi.Application.Dto.SMS;
using PAMAi.Application.Services.Interfaces;
using PAMAi.Domain.Options;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace PAM_Ai.ExternalServices.Services.SMS
{
    public class SmsRepository : ISmsRepository
    {
        private readonly string _baseUrl;
        private readonly string _apiKey;
        private readonly TermiiOptions _settings;
        private readonly ILogger<SmsRepository> _logger;
        public SmsRepository(string baseurl, string apikey, ILogger<SmsRepository> logger, IOptions<TermiiOptions> settings)
        {
            this._baseUrl = baseurl;
            _apiKey = apikey;
            _logger = logger;
            _settings = settings.Value;

        }

        public async Task<Result<SmsResponse>> SendSmsAsync(SmsRequest message)
        {
            if (!PhoneNumberValidator.AreValid(message.To))
            {
                return Result<SmsResponse>.Failure(SMSErrors.PhoneNumberValidation);
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
                    return Result<SmsResponse>.Success(smsResponse);
                }
                else
                {
                    _logger.LogError("Failed to Send SMS to '{ phonenumber }'", message.To);
                    return Result<SmsResponse>.Failure(SMSErrors.SMSFailure);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception: { ex.Message}");
                return Result<SmsResponse>.Failure(SMSErrors.SMSException);
            }
        }
    
    }
}

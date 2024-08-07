﻿using Microsoft.AspNetCore.Http;
using PAMAi.Application;

namespace PAMAi.Infrastructure.ExternalServices.Errors;
internal class SMSErrors
{
    /// <summary>
    /// Issues from Termii
    /// </summary>
    public static readonly Error SMSFailure = new("Failed to send SMS");
    public static readonly Error SMSException = new("Failed to send SMS");
    public static readonly Error PhoneNumberValidation = new("Invalid phone number format.", StatusCodes.Status400BadRequest);
}

﻿using PAMAi.Application;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PAM_Ai.PAMAi.Infrastructure.ExternalServices.Errors;
internal class SMSErrors
{
    /// <summary>
    /// Issues from Termii
    /// </summary>
    public static readonly Error SMSFailure = new("Failed to send SMS");
    public static readonly Error SMSException = new("Failed to send SMS");
    public static readonly Error PhoneNumberValidation = new("Invalid phone number format.");
}

using PAMAi.Application;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PAMAi.Infrastructure.ExternalServices.Errors;
public class FcmErrors
{
    /// <summary>
    /// Issues from Termii
    /// </summary>
    ///  public static readonly Error FcmException = new("Cannoy send push notifications", "Check google implementation again");
    public static readonly Error FcmException = new("Could not send Push Notifications", "Failed to Notifications");
    public static readonly Error FcmFailure = new("Could not send Push Notifications", "Failed to Notifications");
    
    public static readonly Error PhoneNumberValidation = new("Invalid phone number format.", "Numbers must start with '234' and be 13 digits long.");
}
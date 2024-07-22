using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PAM_Ai.ExternalServices.Validation;
public static class PhoneNumberValidator
{
    public static bool IsValid(string phoneNumber)
    {
        return phoneNumber.StartsWith("234") && phoneNumber.Length == 13 && long.TryParse(phoneNumber, out _);
    }

    public static bool AreValid(string phoneNumbers)
    {
        var numbers = phoneNumbers.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
        return numbers.All(IsValid);
    }
}

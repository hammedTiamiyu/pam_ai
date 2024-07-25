namespace PAMAi.Infrastructure.ExternalServices.Validation;
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

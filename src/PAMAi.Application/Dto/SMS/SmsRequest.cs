namespace PAMAi.Application.Dto.SMS;
public class SmsRequest
{
    /// <summary>
    /// Recipeint , Phone No: must be in 2,34 e.g 23456786654 its required, for multiple use Array as in ["234000000", "23455667788"] array takes 100 phone no at a time
    /// </summary>
    public string To { get; set; }
    /// <summary>
    /// Message body
    /// </summary>
    public string Sms { get; set; }

    //public string MediaUrl { get; set; }
    //public string MediaCaption { get; set; }

}

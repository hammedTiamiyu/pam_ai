namespace PAMAi.Application.Dto.Account;
public class PasswordResetRequest
{
    public string PhoneNumber { get; set; }
    public string ResetLink { get; set; }
}
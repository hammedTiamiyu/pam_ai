using PAMAi.Application;

namespace PAMAi.Infrastructure.ExternalServices.Errors;
internal static class EmailServiceErrors
{
    internal static readonly Error MailNotSent = new("Email not sent successfully");
}

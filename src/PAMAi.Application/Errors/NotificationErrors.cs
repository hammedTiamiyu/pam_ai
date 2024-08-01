using PAMAi.Application;

namespace PAMAi.Application.Errors;
internal static class NotificationErrors
{
    internal readonly static Error SendFailed = new("Notifications failed");
}

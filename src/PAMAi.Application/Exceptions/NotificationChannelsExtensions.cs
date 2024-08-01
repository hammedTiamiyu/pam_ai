using PAMAi.Domain.Enums;

namespace PAMAi.Application.Exceptions;
internal static class NotificationChannelsExtensions
{
    /// <summary>
    /// Indicates if the notification channel, <paramref name="channel"/>, is a part of the possibly aggregated
    /// notification channels in <paramref name="left"/>.
    /// </summary>
    /// <param name="left">
    /// <see cref="NotificationChannels"/> to be tested against.
    /// </param>
    /// <param name="channel">
    /// Channel checked for existence.
    /// </param>
    /// <returns>
    /// <see langword="true"/> if <paramref name="left"/> contains <paramref name="channel"/>,
    /// otherwise, <see langword="false"/>.
    /// </returns>
    internal static bool HasChannel(this NotificationChannels left, NotificationChannels channel)
    {
        return (left & channel) != NotificationChannels.None;
    }
}

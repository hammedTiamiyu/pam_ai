namespace PAMAi.Domain.Options;

/// <summary>
/// Email options.
/// </summary>
public class EmailOptions
{
    public static readonly string ConfigurationKey = "Email";

    /// <summary>
    /// Email sender details.
    /// </summary>
    public SenderOptions Sender { get; set; } = new();

    /// <summary>
    /// SMTP settings.
    /// </summary>
    public SmtpOptions Smtp { get; set; } = new();

    /// <summary>
    /// Sender options.
    /// </summary>
    public class SenderOptions
    {
        /// <summary>
        /// Sender's name.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Sender's email.
        /// </summary>
        public string Address { get; set; } = string.Empty;
    }

    /// <summary>
    /// SMTP options.
    /// </summary>
    public class SmtpOptions
    {
        /// <summary>
        /// SMTP host.
        /// </summary>
        public string Host { get; set; } = string.Empty;

        /// <summary>
        /// SMTP username.
        /// </summary>
        public string UserName { get; set; } = string.Empty;

        /// <summary>
        /// SMTP password.
        /// </summary>
        public string Password { get; set; } = string.Empty;

        /// <summary>
        /// SMTP host port.
        /// </summary>
        public int Port { get; set; }
    }
}

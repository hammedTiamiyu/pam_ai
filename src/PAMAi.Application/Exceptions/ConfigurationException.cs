namespace PAMAi.Application.Exceptions;

/// <summary>
/// Exception thrown when there is an issue with the application configuration.
/// </summary>
public class ConfigurationException: Exception
{
    /// <summary>
    /// Creates a new instance of <see cref="ConfigurationException"/>.
    /// </summary>
    /// <param name="message">
    /// Error message.
    /// </param>
    /// <param name="faultyConfigurationName">
    /// Name of the faulty configuration.
    /// </param>
    public ConfigurationException(string? message, string faultyConfigurationName) : base(message)
    {
        FaultyConfiguration = faultyConfigurationName;
    }

    /// <summary>
    /// Name of the invalid configuration.
    /// </summary>
    public string FaultyConfiguration { get; }
}

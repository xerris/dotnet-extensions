namespace Xerris.Extensions.Configuration;

/// <summary>
/// Represents errors that occur during application configuration.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="ConfigurationException" /> class with the specified
/// <paramref name="message" />.
/// </remarks>
/// <param name="message">The configuration error message.</param>
public class ConfigurationException(string message) : Exception(message);

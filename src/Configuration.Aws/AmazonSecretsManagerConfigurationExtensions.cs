using Amazon.Runtime;
using Microsoft.Extensions.Configuration;

namespace Xerris.Extensions.Configuration.Aws;

/// <summary>
/// Extension methods for building configurations with
/// <a href="https://aws.amazon.com/secrets-manager/">AWS Secrets Manager</a>.
/// </summary>
public static class AmazonSecretsManagerConfigurationExtensions
{
    /// <summary>
    /// Add settings from AWS Secrets Manager to the configuration.
    /// </summary>
    /// <param name="configurationBuilder">The <see cref="IConfigurationBuilder" /> to add settings to.</param>
    /// <param name="region">The AWS region.</param>
    /// <param name="secretNames">The secret names to include.</param>
    /// <returns>The <see cref="IConfigurationBuilder" /> instance.</returns>
    public static IConfigurationBuilder AddAmazonSecretsManager(this IConfigurationBuilder configurationBuilder,
        string region, params string[] secretNames)
    {
        var credentials = FallbackCredentialsFactory.GetCredentials();

        return configurationBuilder.AddAmazonSecretsManager(credentials, region, secretNames);
    }

    /// <summary>
    /// Add settings from AWS Secrets Manager to the configuration.
    /// </summary>
    /// <param name="configurationBuilder">The <see cref="IConfigurationBuilder" /> to add settings to.</param>
    /// <param name="credentials">The <see cref="AWSCredentials" /> to use.</param>
    /// <param name="region">The AWS region name.</param>
    /// <param name="secretNames">The secret names to include.</param>
    /// <returns>The <see cref="IConfigurationBuilder" /> instance.</returns>
    public static IConfigurationBuilder AddAmazonSecretsManager(this IConfigurationBuilder configurationBuilder,
        AWSCredentials credentials, string region, params string[] secretNames)
    {
        foreach (var secretName in secretNames)
        {
            var configurationSource = new AmazonSecretsManagerConfigurationSource(credentials, region, secretName);

            configurationBuilder.Add(configurationSource);
        }

        return configurationBuilder;
    }
}

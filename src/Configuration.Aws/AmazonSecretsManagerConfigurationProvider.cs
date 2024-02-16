using System.Text;
using System.Text.Json;
using Amazon;
using Amazon.Runtime;
using Amazon.SecretsManager;
using Amazon.SecretsManager.Model;
using Microsoft.Extensions.Configuration;

namespace Xerris.Extensions.Configuration.Aws;

internal sealed class AmazonSecretsManagerConfigurationProvider(
    AWSCredentials credentials,
    string region,
    string secretName)
    : ConfigurationProvider
{
    public override void Load()
    {
        var secret = GetSecret();

        Data = JsonSerializer.Deserialize<Dictionary<string, string?>>(secret)
            ?? throw new ConfigurationException($"Could not deserialize secret '{secret}' to dictionary");
    }

    private string GetSecret()
    {
        var request = new GetSecretValueRequest
        {
            SecretId = secretName,
            VersionStage = "AWSCURRENT" // VersionStage defaults to AWSCURRENT if unspecified.
        };

        using var client = new AmazonSecretsManagerClient(credentials, RegionEndpoint.GetBySystemName(region));

        var response = client.GetSecretValueAsync(request).Result;

        if (response.SecretString is { } secretString)
            return secretString;

        using var memoryStream = response.SecretBinary;
        using var reader = new StreamReader(memoryStream);
        secretString = Encoding.UTF8.GetString(Convert.FromBase64String(reader.ReadToEnd()));

        return secretString;
    }
}

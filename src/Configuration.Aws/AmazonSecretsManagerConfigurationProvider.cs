using System.Text;
using System.Text.Json;
using Amazon;
using Amazon.Runtime;
using Amazon.SecretsManager;
using Amazon.SecretsManager.Model;
using Microsoft.Extensions.Configuration;

namespace Xerris.Extensions.Configuration.Aws;

internal class AmazonSecretsManagerConfigurationProvider : ConfigurationProvider
{
    private readonly AWSCredentials _credentials;
    private readonly string _region;
    private readonly string _secretName;

    public AmazonSecretsManagerConfigurationProvider(AWSCredentials credentials, string region, string secretName)
    {
        _credentials = credentials;
        _region = region;
        _secretName = secretName;
    }

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
            SecretId = _secretName,
            VersionStage = "AWSCURRENT" // VersionStage defaults to AWSCURRENT if unspecified.
        };

        using var client = new AmazonSecretsManagerClient(_credentials, RegionEndpoint.GetBySystemName(_region));

        var response = client.GetSecretValueAsync(request).Result;

        if (response.SecretString is { } secretString)
            return secretString;

        using var memoryStream = response.SecretBinary;
        using var reader = new StreamReader(memoryStream);
        secretString = Encoding.UTF8.GetString(Convert.FromBase64String(reader.ReadToEnd()));

        return secretString;
    }
}

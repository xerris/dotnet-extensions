using Amazon.Runtime;
using Microsoft.Extensions.Configuration;

namespace Xerris.Extensions.Configuration.Aws;

internal class AmazonSecretsManagerConfigurationSource : IConfigurationSource
{
    private readonly AWSCredentials _credentials;
    private readonly string _region;
    private readonly string _secretName;

    public AmazonSecretsManagerConfigurationSource(AWSCredentials credentials, string region, string secretName)
    {
        _credentials = credentials;
        _region = region;
        _secretName = secretName;
    }

    public IConfigurationProvider Build(IConfigurationBuilder builder)
    {
        return new AmazonSecretsManagerConfigurationProvider(_credentials, _region, _secretName);
    }
}

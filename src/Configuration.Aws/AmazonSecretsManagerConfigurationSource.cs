using Amazon.Runtime;
using Microsoft.Extensions.Configuration;

namespace Xerris.Extensions.Configuration.Aws;

internal class AmazonSecretsManagerConfigurationSource(AWSCredentials credentials, string region, string secretName)
    : IConfigurationSource
{
    public IConfigurationProvider Build(IConfigurationBuilder builder)
    {
        return new AmazonSecretsManagerConfigurationProvider(credentials, region, secretName);
    }
}

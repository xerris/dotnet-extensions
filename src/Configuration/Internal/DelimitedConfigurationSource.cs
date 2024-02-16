using Microsoft.Extensions.Configuration;

namespace Xerris.Extensions.Configuration.Internal;

internal sealed class DelimitedConfigurationSource(IEnumerable<string> keyDelimiters, IConfiguration config)
    : IConfigurationSource
{
    public IConfigurationProvider Build(IConfigurationBuilder builder)
    {
        return new DelimitedConfigurationProvider(keyDelimiters, config);
    }
}

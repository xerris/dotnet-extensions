using Microsoft.Extensions.Configuration;

namespace Xerris.Extensions.Configuration.Tests;

public class ConfigurationBuilderExtensionsTests
{
    [Fact]
    public void SetKeyDelimiters_adds_values_with_new_keys()
    {
        var delimiters = new[] { "-", ":", "_" };

        const string configValue = "baz";

        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?> { { "foo-bar", "baz" } })
            .SetKeyDelimiters(delimiters)
            .Build();

        foreach (var delimiter in delimiters)
            config[$"foo{delimiter}bar"].Should().Be(configValue);
    }
}

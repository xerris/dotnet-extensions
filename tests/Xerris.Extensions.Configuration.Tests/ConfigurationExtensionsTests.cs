using System.Text.Json;
using Microsoft.Extensions.Configuration;

namespace Xerris.Extensions.Configuration.Tests;

public class ConfigurationExtensionsTests
{
    [Fact]
    public void Require_returns_configuration_value()
    {
        const string key = "foo";
        const string value = "bar";

        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?> { { key, value } })
            .Build();

        config.Require(key).Should().Be(value);
    }

    [Fact]
    public void Require_throws_configuration_exception_when_key_is_not_found()
    {
        const string key = "foo";

        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?> { { "bar", "baz" } })
            .Build();

        var test = () => config.Require(key);

        test.Should().Throw<ConfigurationException>().WithMessage($"*{key}*");
    }

    [Theory]
    [InlineData((string?) null)]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("\r\n")]
    public void Require_throws_configuration_exception_when_value_is_null_or_white_space(string? badValue)
    {
        const string key = "foo";

        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?> { { key, badValue } })
            .Build();

        var test = () => config.Require(key);

        test.Should().Throw<ConfigurationException>().WithMessage($"*{key}*");
    }

    [Fact]
    public void Collection_gets_configuration_section_values_as_array()
    {
        var jsonBytes = JsonSerializer.SerializeToUtf8Bytes(new
        {
            section = new
            {
                item1 = 1,
                item2 = 2,
                item3 = 3
            }
        });

        using var jsonStream = new MemoryStream(jsonBytes);

        var config = new ConfigurationBuilder()
            .AddJsonStream(jsonStream)
            .Build();

        var sectionValues = config.Collection("section");

        sectionValues.Should().BeEquivalentTo("1", "2", "3");
    }
}

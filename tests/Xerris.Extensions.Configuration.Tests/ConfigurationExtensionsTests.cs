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
}

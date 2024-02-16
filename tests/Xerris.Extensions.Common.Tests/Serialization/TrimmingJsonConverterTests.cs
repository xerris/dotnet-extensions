using System.Text.Json;
using Xerris.Extensions.Common.Serialization;

namespace Xerris.Extensions.Common.Tests.Serialization;

public class TrimmingJsonConverterTests
{
    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        Converters = { new TrimmingJsonConverter() }
    };

    private static readonly string AsciiWhiteSpaceString = new(
        Enumerable.Range(0, 127)
            .Select(i => (char) i)
            .Where(char.IsWhiteSpace)
            .ToArray());

    [Fact]
    public void Read_removes_leading_and_trailing_whitespace()
    {
        var type = new { value = string.Empty };

        // Convert value to JSON so that whitespace values are necessarily escaped
        var jsonValue = $"{AsciiWhiteSpaceString}foo{AsciiWhiteSpaceString}".ToJson();

        var json = $$"""{"value":{{jsonValue}}}""";

        var deserialized = json.FromJson(type, _jsonOptions);

        deserialized!.value.Should().Be("foo");
    }

    [Fact]
    public void Read_creates_empty_string_for_values_that_are_entirely_whitespace()
    {
        var type = new { value = string.Empty };

        var json = $$"""{"value":{{AsciiWhiteSpaceString.ToJson()}}}""";

        var deserialized = json.FromJson(type, _jsonOptions);

        deserialized!.value.Should().Be(string.Empty);
    }

    [Fact]
    public void Read_keeps_empty_strings()
    {
        var type = new { value = string.Empty };

        var json = $$"""{"value":{{string.Empty.ToJson()}}}""";

        var deserialized = json.FromJson(type, _jsonOptions);

        deserialized!.value.Should().Be(string.Empty);
    }

    [Fact]
    public void Write_removes_leading_and_trailing_whitespace()
    {
        var value = new { value = $"{AsciiWhiteSpaceString}foo{AsciiWhiteSpaceString}" };

        var json = value.ToJson(_jsonOptions);

        json.Should().Be("""{"value":"foo"}""");
    }

    [Fact]
    public void Write_creates_empty_string_for_value_that_is_entirely_whitespace()
    {
        var type = new { value = AsciiWhiteSpaceString };

        var json = type.ToJson(_jsonOptions);

        json.Should().Be("""{"value":""}""");
    }

    [Fact]
    public void Write_keeps_empty_strings()
    {
        var type = new { value = string.Empty };

        var json = type.ToJson(_jsonOptions);

        json.Should().Be("""{"value":""}""");
    }
}

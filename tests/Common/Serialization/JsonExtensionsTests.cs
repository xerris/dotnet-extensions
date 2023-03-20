using System.Text.Json;
using Xerris.Extensions.Common.Serialization;
// ReSharper disable NotAccessedPositionalProperty.Local

namespace Xerris.Extensions.Common.Tests.Serialization;

public class JsonExtensionsTests
{
    private enum SomeEnum
    {
        First
    }

    private record SomeRecord(string Foo, int Bar, SomeEnum Baz);

    [Fact]
    public void ToJson_converts_object_to_single_line_json_string()
    {
        var value = new SomeRecord("foo", 1, SomeEnum.First);

        const string expectedJson = $$"""{"foo":"foo","bar":1,"baz":"First"}""";

        var json = value.ToJson();

        json.Should().Be(expectedJson);
    }

    [Fact]
    public void ToJson_converts_object_to_indented_json_string()
    {
        var value = new SomeRecord("foo", 1, SomeEnum.First);

        const string expectedJson =
            $$"""
            {
              "foo": "foo",
              "bar": 1,
              "baz": "First"
            }
            """;

        var json = value.ToJson(true);

        json.Should().Be(expectedJson);
    }

    [Fact]
    public void ToJson_uses_specified_options()
    {
        var value = new SomeRecord("foo", 1, SomeEnum.First);

        const string expectedJson = $$"""{"Foo":"foo","Bar":1,"Baz":0}""";

        var json = value.ToJson(new JsonSerializerOptions(JsonSerializerDefaults.General));

        json.Should().Be(expectedJson);
    }

    [Fact]
    public void FromJson_reads_valid_json()
    {
        const string json = $$"""{"Foo":"foo","Bar":1,"Baz":0}""";

        var value = json.FromJson<SomeRecord>();

        var expectedValue = new SomeRecord("foo", 1, SomeEnum.First);

        value.Should().BeEquivalentTo(expectedValue);
    }

    [Fact]
    public void FromJson_deserializes_anonymous_type()
    {
        var anonymousType = new { foo = string.Empty };

        const string json = $$"""{"foo":"bar"}""";

        var deserialized = json.FromJson(anonymousType);

        deserialized!.foo.Should().Be("bar");
    }
}

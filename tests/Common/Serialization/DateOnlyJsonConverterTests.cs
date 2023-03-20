using System.Text.Json;
using Xerris.Extensions.Common.Serialization;

namespace Xerris.Extensions.Common.Tests.Serialization;

public class DateOnlyJsonConverterTests
{
    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        Converters = { new DateOnlyJsonConverter() }
    };

    [Fact]
    public void Reads_date_with_time_component()
    {
        var type = new { date = DateOnly.MinValue };

        const string json = $$"""{"date":"2023-01-01T00:00:00.000"}""";

        var deserialized = json.FromJson(type, _jsonOptions);

        deserialized!.date.Should().Be(new DateOnly(2023, 1, 1));
    }

    [Fact]
    public void Reads_date_without_time_component()
    {
        var type = new { date = DateOnly.MinValue };

        const string json = $$"""{"date":"2023-01-01"}""";

        var deserialized = json.FromJson(type, _jsonOptions);

        deserialized!.date.Should().Be(new DateOnly(2023, 1, 1));
    }

    [Fact]
    public void Writes_date_only_property()
    {
        var date = new DateOnly(2023, 1, 1);

        var value = new { date };

        var serialized = value.ToJson(_jsonOptions);

        serialized.Should().Be($$"""{"date":"2023-01-01"}""" );
    }
}

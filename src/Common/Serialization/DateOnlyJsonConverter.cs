using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Xerris.Extensions.Common.Serialization;

/// <summary>
/// Allows a date with a time component to be deserialized as a <see cref="DateOnly"/>.
/// <remarks>
/// This is useful when clients have a flexible date/time type that serializes with it's time component by default. For
/// example, a Typescript <c>Date</c> can accept <c>"2023-01-01"</c>, but will serialize to
/// <c>2023-01-01T00:00:00.000</c>.
/// </remarks>
/// </summary>
public sealed class DateOnlyJsonConverter : JsonConverter<DateOnly>
{
    public override DateOnly Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var date = DateTime.Parse(reader.GetString()!, CultureInfo.InvariantCulture);

        return DateOnly.FromDateTime(date);
    }

    public override DateOnly ReadAsPropertyName(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var date = DateTime.Parse(reader.GetString()!, CultureInfo.InvariantCulture);

        return DateOnly.FromDateTime(date);
    }

    public override void Write(Utf8JsonWriter writer, DateOnly value, JsonSerializerOptions options)
    {
        var isoDate = value.ToString("O", CultureInfo.InvariantCulture);
        writer.WriteStringValue(isoDate);
    }

    public override void WriteAsPropertyName(Utf8JsonWriter writer, DateOnly value, JsonSerializerOptions options)
    {
        var isoDate = value.ToString("O", CultureInfo.InvariantCulture);
        writer.WritePropertyName(isoDate);
    }
}

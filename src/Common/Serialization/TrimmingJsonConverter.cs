using System.Text.Json;
using System.Text.Json.Serialization;

namespace Xerris.Extensions.Common.Serialization;

/// <summary>
/// Trims whitespace from the beginning and end of string values when serializing or deserializing.
/// </summary>
public sealed class TrimmingJsonConverter : JsonConverter<string>
{
    public override string? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return reader.GetString()?.Trim();
    }

    public override void Write(Utf8JsonWriter writer, string value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.Trim());
    }
}

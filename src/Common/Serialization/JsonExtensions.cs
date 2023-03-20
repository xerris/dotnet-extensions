using System.Text.Json;
using System.Text.Json.Serialization;

namespace Xerris.Extensions.Common.Serialization;

public static class JsonExtensions
{
    public static JsonSerializerOptions DefaultJsonSerializerOptions => new(JsonSerializerDefaults.Web)
    {
        Converters = { new JsonStringEnumConverter() }
    };

    /// <summary>
    /// Get a JSON representation of this object.
    /// </summary>
    /// <param name="obj">The object to serialize</param>
    /// <param name="indented">True to output indented JSON</param>
    /// <returns>The JSON string representing this object</returns>
    public static string ToJson(this object obj, bool indented = false)
    {
        var options = DefaultJsonSerializerOptions;
        options.WriteIndented = indented;

        return obj.ToJson(options);
    }

    /// <summary>
    /// Get a JSON representation of this object.
    /// </summary>
    /// <param name="obj">The object to serialize</param>
    /// <param name="options">The serialization options</param>
    /// <returns>The JSON string representing this object</returns>
    public static string ToJson(this object obj, JsonSerializerOptions options)
    {
        return JsonSerializer.Serialize(obj, options);
    }

    /// <summary>
    /// Parses the string representing a single JSON value into a <typeparamref name="TValue" />.
    /// </summary>
    /// <param name="json">JSON text to parse.</param>
    /// <param name="options">Options to control the behavior during parsing.</param>
    /// <returns>A <typeparamref name="TValue" /> representation of the JSON value.</returns>
    public static TValue? FromJson<TValue>(this string json, JsonSerializerOptions? options = null)
    {
        options ??= DefaultJsonSerializerOptions;

        return JsonSerializer.Deserialize<TValue>(json, options);
    }

    /// <summary>
    /// Parses the string representing a single JSON value into a <paramref name="returnType"/>.
    /// </summary>
    /// <param name="json">JSON text to parse.</param>
    /// <param name="returnType">The type of the object to convert to and return.</param>
    /// <param name="options">Options to control the behavior during parsing.</param>
    /// <returns>A <typeparamref name="TValue" /> representation of the JSON value.</returns>
#pragma warning disable IDE0060 // Remove unused parameter
    public static TValue? FromJson<TValue>(this string json, TValue returnType, JsonSerializerOptions? options = default)
#pragma warning restore IDE0060 // Remove unused parameter
    {
        options ??= DefaultJsonSerializerOptions;

        return JsonSerializer.Deserialize<TValue>(json, options);
    }
}

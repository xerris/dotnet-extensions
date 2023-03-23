using System.Text.Json.Serialization;

namespace Xerris.Extensions.Http.OAuth.Internal;

internal abstract record AccessTokenRequest
{
    /// <summary>
    /// The client identifier issued to the client during the registration process.
    /// </summary>
    [JsonPropertyName("client_id")]
    public string ClientId { get; init; } = string.Empty;

    /// <summary>
    /// The <see href="https://datatracker.ietf.org/doc/html/rfc6749#section-3.3">scope</see> of the access request.
    /// </summary>
    [JsonPropertyName("scope")]
    public string Scope { get; init; } = string.Empty;

    /// <summary>
    /// Additional properties to include with the request. For example, to add an <i>audience</i> parameter to the
    /// request:
    /// <code>
    /// var request = new ExampleAccessTokenRequest
    /// {
    ///     AdditionalProperties = new() { { "audience", "some-audience" } }
    /// }
    /// </code>
    /// <remarks>
    /// This dictionary will usually only contain strings, but <see cref="JsonExtensionDataAttribute"/> requires the
    /// property to be a dictionary of <see cref="object"/>s.
    /// </remarks>
    /// </summary>
    [JsonExtensionData]
    public Dictionary<string, object>? AdditionalProperties { get; init; }
}

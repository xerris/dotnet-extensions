using System.Text.Json.Serialization;

namespace Xerris.Extensions.Http.OAuth;

/// <summary>
/// A successful access token response from the authorization server.
/// <remarks>
/// See the <see href="https://datatracker.ietf.org/doc/html/rfc6749#section-5.1">OAuth 2.0 spec</see> for more
/// information.
/// </remarks>
/// </summary>
public record AccessTokenResponse
{
    /// <summary>
    /// The access token issued by the authorization server.
    /// </summary>
    [JsonPropertyName("access_token")]
    public string AccessToken { get; init; } = string.Empty;

    /// <summary>
    /// The type of the token issued.
    /// </summary>
    [JsonPropertyName("token_type")]
    public string TokenType { get; init; } = string.Empty;

    /// <summary>
    /// The lifetime in seconds of the access token. For example, the value "3600" denotes that the access token will
    /// expire in one hour from the time the response was generated.
    /// </summary>
    [JsonPropertyName("expires_in")]
    public int ExpiresIn { get; init; }

    /// <summary>
    /// The scope of the access token requested by the client.
    /// </summary>
    [JsonPropertyName("scope")]
    public string? Scope { get; init; }

    /// <summary>
    /// Additional properties in the response from the authorization server.
    /// <remarks>
    /// This dictionary <i>should</i> only contain strings, but <see cref="JsonExtensionDataAttribute"/> requires the
    /// property to be a dictionary of <see cref="object"/>s.
    /// </remarks>
    /// </summary>
    [JsonExtensionData]
    public Dictionary<string, object>? AdditionalProperties { get; init; }
}

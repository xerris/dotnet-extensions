using System.Text.Json.Serialization;

namespace Xerris.Extensions.Http.OAuth.Internal;

/// <summary>
/// Access token request for the
/// <see href="https://datatracker.ietf.org/doc/html/rfc6749#section-4.4">client credentials</see> grant type.
/// </summary>
internal record ClientCredentialsAccessTokenRequest : AccessTokenRequest
{
    /// <summary>
    /// The client secret.
    /// </summary>
    [JsonPropertyName("client_secret")]
    public string ClientSecret { get; init; } = string.Empty;

    /// <summary>
    /// The grant type.
    /// </summary>
    [JsonPropertyName("grant_type")]
    public string GrantType => "client_credentials";
}

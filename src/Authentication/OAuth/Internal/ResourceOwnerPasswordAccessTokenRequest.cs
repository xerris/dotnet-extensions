using System.Text.Json.Serialization;

namespace Xerris.Extensions.Authentication.OAuth.Internal;

/// <summary>
/// Access token request for the
/// <see href="https://datatracker.ietf.org/doc/html/rfc6749#section-4.3">client credentials</see> grant type.
/// <remarks>
/// This grant type should <b>only</b> be used when there is a high degree of trust between the resource owner and the
/// client (e.g., the client is part of the device operating system or a highly privileged application), and when other
/// authorization grant types are not available (such as an authorization code).
/// </remarks>
/// </summary>
internal record ResourceOwnerPasswordAccessTokenRequest : AccessTokenRequest
{
    /// <summary>
    /// The resource owner username.
    /// </summary>
    [JsonPropertyName("username")]
    public string Username { get; init; } = string.Empty;

    /// <summary>
    /// The resource owner password.
    /// </summary>
    [JsonPropertyName("password")]
    public string Password { get; init; } = string.Empty;

    /// <summary>
    /// The client secret.
    /// </summary>
    [JsonPropertyName("client_secret")]
    public string ClientSecret { get; init; } = string.Empty;

    /// <summary>
    /// The grant type.
    /// </summary>
    [JsonPropertyName("grant_type")]
    public virtual string GrantType => "password";
}

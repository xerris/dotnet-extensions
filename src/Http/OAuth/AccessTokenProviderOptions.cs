namespace Xerris.Extensions.Http.OAuth;

/// <summary>
/// The configuration values used to acquire access tokens.
/// </summary>
public abstract record AccessTokenProviderOptions
{
    /// <summary>
    /// The endpoint used by the client to obtain an access token by presenting its authorization grant or refresh
    /// token.
    /// </summary>
    public Uri TokenEndpoint { get; set; } = null!;

    /// <summary>
    /// The client identifier issued to the client during the registration process.
    /// </summary>
    public string ClientId { get; set; } = string.Empty;

    /// <summary>
    /// Additional properties to include with access token requests. For example, to add a parameter for
    /// <i>audience</i> to the request:
    /// <code>
    /// AdditionalProperties = new() { { "audience", "some-audience" } }
    /// </code>
    /// </summary>
    public Dictionary<string, string>? AdditionalProperties { get; set; }
}

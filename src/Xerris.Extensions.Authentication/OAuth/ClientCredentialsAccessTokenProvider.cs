using System.Net.Http.Json;
using Microsoft.Extensions.Options;
using Xerris.Extensions.Authentication.OAuth.Internal;

namespace Xerris.Extensions.Authentication.OAuth;

/// <summary>
/// The options values for acquiring access tokens using the
/// <see href="https://datatracker.ietf.org/doc/html/rfc6749#section-1.3.4">client credentials</see> flow.
/// </summary>
public record ClientCredentialsProviderOptions : AccessTokenProviderOptions, IOptions<ClientCredentialsProviderOptions>
{
    /// <summary>
    /// The client secret.
    /// </summary>
    public string ClientSecret { get; set; } = string.Empty;

    /// <summary>
    /// The default configured M<see cref="ClientCredentialsProviderOptions" /> instance.
    /// </summary>
    public ClientCredentialsProviderOptions Value => this;
}

/// <summary>
/// Acquires an OAuth 2.0 access token using the
/// <see href="https://datatracker.ietf.org/doc/html/rfc6749#section-1.3.4">client credentials</see> flow.
/// </summary>
/// <remarks>
/// Create a new instance of <see cref="ClientCredentialsAccessTokenProvider" />.
/// </remarks>
/// <param name="httpClient">The <see cref="HttpClient" /> to use.</param>
/// <param name="options">The configuration options for the access token request.</param>
public class ClientCredentialsAccessTokenProvider(HttpClient httpClient,
    IOptions<ClientCredentialsProviderOptions> options) : IAccessTokenProvider
{
    private readonly ClientCredentialsProviderOptions _options = options.Value;

    /// <inheritdoc />
    public async Task<AccessTokenResponse> GetAccessTokenAsync(params string[] scopes)
    {
        var scopesValue = string.Join(" ", scopes);

        var request = new ClientCredentialsAccessTokenRequest
        {
            ClientId = _options.ClientId,
            ClientSecret = _options.ClientSecret,
            Scope = scopesValue,
            AdditionalProperties = _options.AdditionalProperties?.ToDictionary(k => k.Key, v => (object) v.Value)
        };

        using var response = await httpClient.PostAsJsonAsync(_options.TokenEndpoint, request).ConfigureAwait(false);

        try
        {
            response.EnsureSuccessStatusCode();
        }
        catch (HttpRequestException ex)
        {
            throw new HttpRequestException($"Failed to retrieve access token from {_options.TokenEndpoint} " +
                $"for client {_options.ClientId} using grant type {request.GrantType}", ex);
        }

        var accessTokenResponse = await response.Content.ReadFromJsonAsync<AccessTokenResponse>().ConfigureAwait(false);

        return accessTokenResponse!;
    }
}

using System.Net.Http.Headers;
using Xerris.Extensions.Http.OAuth;

namespace Xerris.Extensions.Http;

/// <summary>
/// Adds OAuth 2.0 <see href="https://datatracker.ietf.org/doc/html/rfc6750">bearer token authentication</see> to
/// outgoing HTTP requests.
/// </summary>
public class BearerTokenAuthenticationHandler : DelegatingHandler
{
    private readonly IAccessTokenProvider _accessTokenProvider;
    private readonly string[] _scopes;

    /// <summary>
    /// Initializes a new instance of the <see cref="BearerTokenAuthenticationHandler"/> class.
    /// </summary>
    /// <param name="accessTokenProvider">The <see cref="IAccessTokenProvider"/> to acquire bearer tokens with.</param>
    /// <param name="scopes">
    /// The <see href="https://datatracker.ietf.org/doc/html/rfc6749#section-3.3">scopes</see> to use when requesting
    /// access tokens.
    /// </param>
    public BearerTokenAuthenticationHandler(IAccessTokenProvider accessTokenProvider, params string[] scopes)
    {
        _accessTokenProvider = accessTokenProvider;
        _scopes = scopes;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        var response = await _accessTokenProvider.GetAccessTokenAsync(_scopes);

        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", response.AccessToken);

        return await base.SendAsync(request, cancellationToken);
    }
}

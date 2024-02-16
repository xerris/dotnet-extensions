using System.Net.Http.Headers;

namespace Xerris.Extensions.Authentication.OAuth;

/// <summary>
/// Adds OAuth 2.0 <see href="https://datatracker.ietf.org/doc/html/rfc6750">bearer token authentication</see> to
/// outgoing HTTP requests.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="BearerTokenAuthenticationHandler" /> class.
/// </remarks>
/// <param name="accessTokenProvider">The <see cref="IAccessTokenProvider" /> to acquire bearer tokens with.</param>
/// <param name="scopes">
/// The <see href="https://datatracker.ietf.org/doc/html/rfc6749#section-3.3">scopes</see> to use when requesting
/// access tokens.
/// </param>
public class BearerTokenAuthenticationHandler(IAccessTokenProvider accessTokenProvider, params string[] scopes)
    : DelegatingHandler
{
    /// <inheritdoc />
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        var response = await accessTokenProvider.GetAccessTokenAsync(scopes).ConfigureAwait(false);

        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", response.AccessToken);

        return await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
    }
}

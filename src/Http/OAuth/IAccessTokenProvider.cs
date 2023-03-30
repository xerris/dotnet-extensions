namespace Xerris.Extensions.Http.OAuth;

public interface IAccessTokenProvider
{
    /// <summary>
    /// Request an <see href="https://oauth.net/2/access-tokens/">OAuth2 access token</see>.
    /// </summary>
    /// <param name="scopes">The <see href="https://oauth.net/2/scope/">scopes</see> to include in the request.</param>
    /// <returns>The access token response.</returns>
    Task<AccessTokenResponse> GetAccessTokenAsync(params string[] scopes);
}

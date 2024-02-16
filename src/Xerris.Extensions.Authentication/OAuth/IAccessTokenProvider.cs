namespace Xerris.Extensions.Authentication.OAuth;

/// <summary>
/// A service for retrieving <see href="https://oauth.net/2/access-tokens/">OAuth 2.0 access tokens</see>.
/// </summary>
public interface IAccessTokenProvider
{
    /// <summary>
    /// Request an <see href="https://oauth.net/2/access-tokens/">OAuth 2.0 access token</see>.
    /// </summary>
    /// <param name="scopes">The <see href="https://oauth.net/2/scope/">scopes</see> to include in the request.</param>
    /// <returns>The access token response.</returns>
    Task<AccessTokenResponse> GetAccessTokenAsync(params string[] scopes);
}

using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using Xerris.Extensions.Common.Serialization;

namespace Xerris.Extensions.Authentication.OAuth;

/// <summary>
/// A <see cref="IAccessTokenProvider" /> decorator that caches access token responses in a
/// <see cref="IDistributedCache" />.
/// </summary>
/// <remarks>
/// Create a new instance of <see cref="DistributedCachingAccessTokenProvider" />.
/// </remarks>
/// <param name="innerProvider">The underlying <see cref="IAccessTokenProvider" />to decorate.</param>
/// <param name="cache">The <see cref="IDistributedCache" /> to cache responses with.</param>
/// <param name="options">The access token caching configuration options.</param>
public class DistributedCachingAccessTokenProvider(
    IAccessTokenProvider innerProvider,
    IDistributedCache cache,
    IOptions<AccessTokenCachingOptions> options)
    : IAccessTokenProvider
{
    private readonly AccessTokenCachingOptions _options = options.Value;

    /// <summary>
    /// Checks for an unexpired <see cref="AccessTokenResponse" /> with the specified scopes in the cache and returns
    /// it if found. Otherwise, requests a fresh access token from the decorated <see cref="IAccessTokenProvider" />,
    /// stores it, and returns the fresh <see cref="AccessTokenResponse" />.
    /// </summary>
    /// <param name="scopes">The <see href="https://oauth.net/2/scope/">scopes</see> to include in the request.</param>
    /// <returns>The cached <see cref="AccessTokenResponse" /> if found, otherwise the fresh response.</returns>
    public async Task<AccessTokenResponse> GetAccessTokenAsync(params string[] scopes)
    {
        var cacheKeyBase = innerProvider.GetType().Name;
        var cacheKeyId = string.Join("#", scopes);

        var cacheKey = $"{cacheKeyBase}:{cacheKeyId}";

        var cachedAccessTokenResponseValue = await cache.GetStringAsync(cacheKey).ConfigureAwait(false);

        if (!string.IsNullOrEmpty(cachedAccessTokenResponseValue))
            return cachedAccessTokenResponseValue.FromJson<AccessTokenResponse>()!;

        var freshAccessTokenResponse = await innerProvider.GetAccessTokenAsync(scopes).ConfigureAwait(false);
        var freshAccessTokenResponseValue = freshAccessTokenResponse.ToJson();

        var absoluteExpirationRelativeToNow = TimeSpan.FromSeconds(freshAccessTokenResponse.ExpiresIn)
            .Subtract(_options.ExpirationBuffer);

        if (absoluteExpirationRelativeToNow <= TimeSpan.Zero)
        {
            throw new InvalidOperationException(
                $"The calculated relative expiration value of the cache entry must not be negative (current " +
                $"value: {absoluteExpirationRelativeToNow}). Either decrease the expiration buffer or increase " +
                "the expiry time of the access token provided by the authorization server.");
        }

        var cacheEntryOptions = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = absoluteExpirationRelativeToNow
        };

        await cache.SetStringAsync(cacheKey, freshAccessTokenResponseValue, cacheEntryOptions).ConfigureAwait(false);

        return freshAccessTokenResponse;
    }
}

using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using Xerris.Extensions.Common.Serialization;

namespace Xerris.Extensions.Http.OAuth;

/// <summary>
/// An <see cref="IAccessTokenProvider" /> decorator that caches access token responses in an
/// <see cref="IDistributedCache" />.
/// </summary>
public class DistributedCachingAccessTokenProvider : IAccessTokenProvider
{
    private readonly IDistributedCache _cache;
    private readonly IAccessTokenProvider _innerProvider;
    private readonly AccessTokenCachingOptions _options;

    /// <summary>
    /// Create a new instance of <see cref="DistributedCachingAccessTokenProvider" />.
    /// </summary>
    /// <param name="innerProvider">The underlying <see cref="IAccessTokenProvider" />to decorate.</param>
    /// <param name="cache">The <see cref="IDistributedCache" /> to cache responses with.</param>
    /// <param name="options">The access token caching configuration options.</param>
    public DistributedCachingAccessTokenProvider(IAccessTokenProvider innerProvider, IDistributedCache cache,
        IOptions<AccessTokenCachingOptions> options)
    {
        _innerProvider = innerProvider;
        _cache = cache;
        _options = options.Value;
    }

    /// <summary>
    /// Checks for an unexpired <see cref="AccessTokenResponse" /> with the specified scopes in the cache and returns
    /// it if found. Otherwise, requests a fresh access token from the decorated <see cref="IAccessTokenProvider" />,
    /// stores it, and returns the fresh <see cref="AccessTokenResponse" />.
    /// </summary>
    /// <param name="scopes">The <see href="https://oauth.net/2/scope/">scopes</see> to include in the request.</param>
    /// <returns>The cached <see cref="AccessTokenResponse" /> if found, otherwise the fresh response.</returns>
    public async Task<AccessTokenResponse> GetAccessTokenAsync(params string[] scopes)
    {
        var cacheKeyBase = _innerProvider.GetType().Name;
        var cacheKeyId = string.Join("#", scopes);

        var cacheKey = $"{cacheKeyBase}:{cacheKeyId}";

        var cachedAccessTokenResponseValue = await _cache.GetStringAsync(cacheKey).ConfigureAwait(false);

        if (!string.IsNullOrEmpty(cachedAccessTokenResponseValue))
            return cachedAccessTokenResponseValue.FromJson<AccessTokenResponse>()!;

        var freshAccessTokenResponse = await _innerProvider.GetAccessTokenAsync(scopes).ConfigureAwait(false);
        var freshAccessTokenResponseValue = freshAccessTokenResponse.ToJson();

        var cacheEntryOptions = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(freshAccessTokenResponse.ExpiresIn)
                .Subtract(_options.ExpirationBuffer)
        };

        await _cache.SetStringAsync(cacheKey, freshAccessTokenResponseValue, cacheEntryOptions).ConfigureAwait(false);

        return freshAccessTokenResponse;
    }
}

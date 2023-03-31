using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace Xerris.Extensions.Http.OAuth;

public class InMemoryCachingAccessTokenProvider : IAccessTokenProvider
{
    private readonly IAccessTokenProvider _innerProvider;
    private readonly IMemoryCache _cache;
    private readonly AccessTokenCachingOptions _options;

    public InMemoryCachingAccessTokenProvider(IAccessTokenProvider innerProvider, IMemoryCache cache,
        IOptions<AccessTokenCachingOptions> options)
    {
        _innerProvider = innerProvider;
        _cache = cache;
        _options = options.Value;
    }

    public async Task<AccessTokenResponse> GetAccessTokenAsync(params string[] scopes)
    {
        var cacheKeyBase = _innerProvider.GetType().Name;
        var cacheKeyId = string.Join("#", scopes);

        var cacheKey = $"{cacheKeyBase}:{cacheKeyId}";

        var accessTokenResponse = await _cache.GetOrCreateAsync(cacheKey, async entry =>
        {
            var freshAccessTokenResponse = await _innerProvider.GetAccessTokenAsync(cacheKeyId).ConfigureAwait(false);

            var absoluteExpirationRelativeToNow = TimeSpan.FromSeconds(freshAccessTokenResponse.ExpiresIn)
                .Subtract(_options.ExpirationBuffer);

            if (absoluteExpirationRelativeToNow <= TimeSpan.Zero)
            {
                throw new InvalidOperationException(
                    "The calculated relative expiration value of the cache entry must be positive. Either decrease " +
                    "the expiration buffer or increase the expiry time of the access token provided by the " +
                    "authorization server.");
            }

            entry.Value = freshAccessTokenResponse;
            entry.AbsoluteExpirationRelativeToNow = absoluteExpirationRelativeToNow;

            return freshAccessTokenResponse;
        }).ConfigureAwait(false);

        return accessTokenResponse!;
    }
}

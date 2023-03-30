using Microsoft.Extensions.Caching.Distributed;
using Xerris.Extensions.Common.Serialization;

namespace Xerris.Extensions.Http.OAuth;

public class DistributedCachingAccessTokenProvider : IAccessTokenProvider
{
    private readonly IAccessTokenProvider _innerProvider;
    private readonly IDistributedCache _cache;
    private readonly AccessTokenCachingOptions _options;

    public DistributedCachingAccessTokenProvider(IAccessTokenProvider innerProvider, IDistributedCache cache,
        AccessTokenCachingOptions options)
    {
        _innerProvider = innerProvider;
        _cache = cache;
        _options = options;
    }

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

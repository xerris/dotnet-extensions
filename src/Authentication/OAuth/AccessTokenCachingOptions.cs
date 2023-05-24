using Microsoft.Extensions.Options;

namespace Xerris.Extensions.Authentication.OAuth;

/// <summary>
/// Options for configuring access token caching behavior.
/// </summary>
public record AccessTokenCachingOptions : IOptions<AccessTokenCachingOptions>
{
    /// <summary>
    /// The amount of time by which the cache expiration time is advanced to ensure that the cached
    /// <see cref="AccessTokenResponse" /> is not returned after it has actually expired and is no longer valid. This
    /// accounts for factors such as network latency or processing delays (for example, if I cached token expires
    /// <i>during</i> the processing of a web request. Defaults to one minute.
    /// </summary>
    public TimeSpan ExpirationBuffer { get; set; } = TimeSpan.FromMinutes(1);

    /// <summary>
    /// The default configured M<see cref="AccessTokenCachingOptions" /> instance.
    /// </summary>
    public AccessTokenCachingOptions Value => this;
}

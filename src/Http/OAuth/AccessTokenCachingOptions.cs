namespace Xerris.Extensions.Http.OAuth;

public record AccessTokenCachingOptions
{
    /// <summary>
    /// The amount of time by which the cache expiration time is advanced to ensure that the cached
    /// <see cref="AccessTokenResponse"/> is not returned after it has actually expired and is no longer valid. This
    /// accounts for factors such as network latency or processing delays. Defaults to one minute.
    /// </summary>
    public TimeSpan ExpirationBuffer { get; set; } = TimeSpan.FromMinutes(1);
}

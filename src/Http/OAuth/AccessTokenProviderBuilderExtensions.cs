using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;

namespace Xerris.Extensions.Http.OAuth;

/// <summary>
/// Extension methods for <see cref="IAccessTokenProviderBuilder" />.
/// </summary>
public static class AccessTokenProviderBuilderExtensions
{
    /// <summary>
    /// Adds an <see cref="IAccessTokenProvider" /> and related services to the <see cref="IServiceCollection" /> that
    /// acquires access tokens using the OAuth 2.0
    /// <see href="https://datatracker.ietf.org/doc/html/rfc6749#section-1.3.4">client credentials</see> flow.
    /// </summary>
    /// <param name="builder">The <see cref="IAccessTokenProviderBuilder" /> to use.</param>
    /// <param name="options">The options to configure the provider with.</param>
    /// <returns>The <see cref="IAccessTokenProviderBuilder" /> instance.</returns>
    public static IAccessTokenProviderBuilder UseClientCredentialsFlow(
        this IAccessTokenProviderBuilder builder, ClientCredentialsProviderOptions options)
    {
        return builder.UseClientCredentialsFlow(opts =>
        {
            opts.TokenEndpoint = options.TokenEndpoint;
            opts.ClientId = options.ClientId;
            opts.ClientSecret = options.ClientSecret;
            opts.AdditionalProperties = options.AdditionalProperties;
        });
    }

    /// <summary>
    /// Adds an <see cref="IAccessTokenProvider" /> and related services to the <see cref="IServiceCollection" /> that
    /// acquires access tokens using the OAuth 2.0
    /// <see href="https://datatracker.ietf.org/doc/html/rfc6749#section-1.3.4">client credentials</see> flow.
    /// </summary>
    /// <param name="builder">The <see cref="IAccessTokenProviderBuilder" /> to use.</param>
    /// <param name="configure">
    /// A delegate to configure the <see cref="ClientCredentialsProviderOptions" /> for the provider.
    /// </param>
    /// <returns>The <see cref="IAccessTokenProviderBuilder" /> instance.</returns>
    public static IAccessTokenProviderBuilder UseClientCredentialsFlow(
        this IAccessTokenProviderBuilder builder, Action<ClientCredentialsProviderOptions> configure)
    {
        builder.Services.AddOptions<ClientCredentialsProviderOptions>().Configure(configure);

        builder.Services.AddHttpClient<IAccessTokenProvider, ClientCredentialsAccessTokenProvider>();

        return builder;
    }

    /// <summary>
    /// Adds an <see cref="IAccessTokenProvider" /> and related services to the <see cref="IServiceCollection" /> that
    /// acquires access tokens using the the OAuth2.0
    /// <see href="https://datatracker.ietf.org/doc/html/rfc6749#section-1.3.3">resource owner password credentials</see>
    /// flow.
    /// </summary>
    /// <param name="builder">The <see cref="IAccessTokenProviderBuilder" /> to use.</param>
    /// <param name="options">The options to configure the provider with.</param>
    /// <returns>The <see cref="IAccessTokenProviderBuilder" /> instance.</returns>
    public static IAccessTokenProviderBuilder UseResourceOwnerPasswordFlow(
        this IAccessTokenProviderBuilder builder, ResourceOwnerPasswordProviderOptions options)
    {
        return builder.UseResourceOwnerPasswordFlow(opts =>
        {
            opts.TokenEndpoint = options.TokenEndpoint;
            opts.ClientId = options.ClientId;
            opts.ClientSecret = options.ClientSecret;
            opts.Username = options.Username;
            opts.Password = options.Password;
            opts.AdditionalProperties = options.AdditionalProperties;
        });
    }

    /// <summary>
    /// Adds an <see cref="IAccessTokenProvider" /> and related services to the <see cref="IServiceCollection" /> that
    /// acquires access tokens using the the OAuth2.0
    /// <see href="https://datatracker.ietf.org/doc/html/rfc6749#section-1.3.3">resource owner password credentials</see>
    /// flow.
    /// </summary>
    /// <param name="builder">The <see cref="IAccessTokenProviderBuilder" /> to use.</param>
    /// <param name="configure">
    /// A delegate to configure the <see cref="ResourceOwnerPasswordProviderOptions" /> for the provider.
    /// </param>
    /// <returns>The <see cref="IAccessTokenProviderBuilder" /> instance.</returns>
    public static IAccessTokenProviderBuilder UseResourceOwnerPasswordFlow(
        this IAccessTokenProviderBuilder builder, Action<ResourceOwnerPasswordProviderOptions> configure)
    {
        builder.Services.AddOptions<ResourceOwnerPasswordProviderOptions>().Configure(configure);
        builder.Services.AddHttpClient<IAccessTokenProvider, ResourceOwnerPasswordAccessTokenProvider>();

        return builder;
    }

    /// <summary>
    /// Adds a custom implementation of <see cref="IAccessTokenProvider" /> to the service collection.
    /// </summary>
    /// <param name="builder">The <see cref="IAccessTokenProviderBuilder" /> to use.</param>
    /// <param name="provider">The custom <see cref="IAccessTokenProvider" />.</param>
    /// <returns>The <see cref="IAccessTokenProviderBuilder" /> instance.</returns>
    public static IAccessTokenProviderBuilder UseCustomProvider(this IAccessTokenProviderBuilder builder,
        IAccessTokenProvider provider)
    {
        builder.Services.AddTransient(_ => provider);

        return builder;
    }

    /// <summary>
    /// Adds an <see cref="IAccessTokenProvider" /> and related services to the <see cref="IServiceCollection" /> that
    /// caches access token responses using an <see cref="IDistributedCache" />.
    /// </summary>
    /// <remarks>
    /// If no distributed cache is configured in the service collection, cache entries will be stored using an
    /// in-memory distributed cache. Separate configuration of a different distributed cache will override this
    /// behavior.
    /// </remarks>
    /// <see cref="MemoryDistributedCache" />
    /// <seealso cref="MemoryCacheServiceCollectionExtensions.AddDistributedMemoryCache(IServiceCollection)" />
    /// <param name="builder">The <see cref="IAccessTokenProviderBuilder" /> to use.</param>
    /// <param name="configure">
    /// A delegate to configure the <see cref="AccessTokenCachingOptions" /> for the provider.
    /// </param>
    /// <returns>The <see cref="IAccessTokenProviderBuilder" /> instance.</returns>
    public static IAccessTokenProviderBuilder WithDistributedCaching(this IAccessTokenProviderBuilder builder,
        Action<AccessTokenCachingOptions>? configure = null)
    {
        builder.Services.AddOptions<AccessTokenCachingOptions>().Configure(configure ?? (_ => { }));

        builder.Services.AddDistributedMemoryCache();
        builder.Services.Decorate<IAccessTokenProvider, DistributedCachingAccessTokenProvider>();

        return builder;
    }

    /// <summary>
    /// Configures an in-memory caching mechanism for the access token provider.
    /// </summary>
    /// <param name="builder">The <see cref="IAccessTokenProviderBuilder" /> to use.</param>
    /// <param name="configure">
    /// Optional delegate to configure the <see cref="AccessTokenCachingOptions" /> for the provider.
    /// </param>
    /// <param name="configureMemoryCache">Optional delegate to configure the memory cache options.</param>
    /// <returns>The <see cref="IAccessTokenProviderBuilder" /> instance.</returns>
    public static IAccessTokenProviderBuilder WithInMemoryCaching(this IAccessTokenProviderBuilder builder,
        Action<AccessTokenCachingOptions>? configure = null, Action<MemoryCacheOptions>? configureMemoryCache = null)
    {
        builder.Services.AddOptions<AccessTokenCachingOptions>().Configure(configure ?? (_ => { }));

        builder.Services.AddMemoryCache(configureMemoryCache ?? (_ => { }));
        builder.Services.Decorate<IAccessTokenProvider, InMemoryCachingAccessTokenProvider>();

        return builder;
    }
}

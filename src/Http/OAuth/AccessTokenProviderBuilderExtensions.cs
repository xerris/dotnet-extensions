using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;

namespace Xerris.Extensions.Http.OAuth;

/// <summary>
/// Extension methods for <see cref="IAccessTokenProviderBuilder"/>.
/// </summary>
public static class AccessTokenProviderBuilderExtensions
{
    /// <summary>Add </summary>
    ///  <param name="builder">The <see cref="IAccessTokenProviderBuilder"/> to use.</param>
    ///  <param name="options">The options to configure the provider with.</param>
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

    ///<summary>todo</summary>
    /// <param name="builder">The <see cref="IAccessTokenProviderBuilder"/> to use.</param>
    /// <param name="configure">
    /// A delegate to configure the <see cref="ClientCredentialsProviderOptions"/> for the provider.
    /// </param>
    public static IAccessTokenProviderBuilder UseClientCredentialsFlow(
        this IAccessTokenProviderBuilder builder, Action<ClientCredentialsProviderOptions> configure)
    {
        builder.Services.AddOptions<ClientCredentialsProviderOptions>().Configure(configure);

        builder.Services.AddHttpClient<IAccessTokenProvider, ClientCredentialsAccessTokenProvider>();

        return builder;
    }

    /// <param name="builder">The <see cref="IAccessTokenProviderBuilder"/> to use.</param>
    /// <param name="options">A delegate to configure the <see cref="ClientCredentialsProviderOptions"/> for the provider.</param>
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

    /// <param name="builder">The <see cref="IAccessTokenProviderBuilder"/> to use.</param>
    /// <param name="configure">
    /// A delegate to configure the <see cref="ResourceOwnerPasswordProviderOptions"/> for the provider.
    /// </param>
    public static IAccessTokenProviderBuilder UseResourceOwnerPasswordFlow(
        this IAccessTokenProviderBuilder builder, Action<ResourceOwnerPasswordProviderOptions> configure)
    {
        builder.Services.AddOptions<ResourceOwnerPasswordProviderOptions>().Configure(configure);
        builder.Services.AddHttpClient<IAccessTokenProvider, ResourceOwnerPasswordAccessTokenProvider>();

        return builder;
    }

    /// <summary>
    /// Adds a custom implementation of <see cref="IAccessTokenProvider"/> to the service collection.
    /// </summary>
    /// <param name="builder">The <see cref="IAccessTokenProviderBuilder"/> to use.</param>
    /// <param name="provider">The custom <see cref="IAccessTokenProvider"/>.</param>
    public static IAccessTokenProviderBuilder UseCustomProvider(this IAccessTokenProviderBuilder builder,
        IAccessTokenProvider provider)
    {
        builder.Services.AddTransient(_ => provider);

        return builder;
    }

    /// <param name="builder">The <see cref="IAccessTokenProviderBuilder"/> to use.</param>
    public static IAccessTokenProviderBuilder WithDistributedCaching(this IAccessTokenProviderBuilder builder,
        Action<AccessTokenCachingOptions>? configure = null)
    {
        builder.Services.AddOptions<AccessTokenCachingOptions>().Configure(configure ?? (_ => { }));
        
        builder.Services.AddDistributedMemoryCache();
        builder.Services.Decorate<IAccessTokenProvider, DistributedCachingAccessTokenProvider>();

        return builder;
    }

    /// <param name="builder">The <see cref="IAccessTokenProviderBuilder"/> to use.</param>
    public static IAccessTokenProviderBuilder WithInMemoryCaching(this IAccessTokenProviderBuilder builder,
        Action<AccessTokenCachingOptions>? configure = null, Action<MemoryCacheOptions>? configureMemoryCache = null)
    {
        builder.Services.AddOptions<AccessTokenCachingOptions>().Configure(configure ?? (_ => { }));

        builder.Services.AddMemoryCache(configureMemoryCache ?? (_ => { }));
        builder.Services.Decorate<IAccessTokenProvider, InMemoryCachingAccessTokenProvider>();

        return builder;
    }
}

using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;

namespace Xerris.Extensions.Http.OAuth;

public static class AccessTokenProviderBuilderExtensions
{
    public static IAccessTokenProviderBuilder UseClientCredentialsFlow(
        this IAccessTokenProviderBuilder builder, ClientCredentialsProviderOptions options)
    {
        builder.Services.AddOptions(); // todo: why am I doing this
        builder.Services.AddTransient(_ => options);
        builder.Services.AddHttpClient<IAccessTokenProvider, ClientCredentialsAccessTokenProvider>();

        return builder;
    }

    public static IAccessTokenProviderBuilder UseClientCredentialsFlow(
        this IAccessTokenProviderBuilder builder,
        Action<ClientCredentialsProviderOptions> clientCredentialsOptionsAction)
    {
        builder.Services.AddOptions();//todo: why am I doing this
        builder.Services.AddHttpClient<IAccessTokenProvider, ResourceOwnerPasswordAccessTokenProvider>();

        return builder;
    }
    public static IAccessTokenProviderBuilder UseResourceOwnerPasswordFlow(
        this IAccessTokenProviderBuilder builder, ResourceOwnerPasswordProviderOptions options)
    {
        return builder;
    }

    public static IAccessTokenProviderBuilder UseResourceOwnerPasswordFlow(
        this IAccessTokenProviderBuilder builder,
        Action<ResourceOwnerPasswordProviderOptions> resourceOwnerPasswordOptionsAction)
    {
        return builder;
    }

    public static IAccessTokenProviderBuilder UseCustomProvider(this IAccessTokenProviderBuilder builder,
        IAccessTokenProvider provider)
    {
        builder.Services.AddTransient(_ => provider);

        return builder;
    }

    public static IAccessTokenProviderBuilder UseCustomProvider<TProvider>(this IAccessTokenProviderBuilder builder)
        where TProvider : class, IAccessTokenProvider
    {
        builder.Services.AddTransient<IAccessTokenProvider, TProvider>();

        return builder;
    }
    public static IAccessTokenProviderBuilder WithDistributedCaching(this IAccessTokenProviderBuilder builder,
        AccessTokenCachingOptions? cachingOptions = null)
    {
        builder.Services.AddTransient(_ => cachingOptions ?? new()); // todo: use options?
        builder.Services.AddDistributedMemoryCache();
        builder.Services.Decorate<IAccessTokenProvider, DistributedCachingAccessTokenProvider>();

        return builder;
    }

    public static IAccessTokenProviderBuilder WithInMemoryCaching(this IAccessTokenProviderBuilder builder,
        AccessTokenCachingOptions? cachingOptions = null, Action<MemoryCacheOptions>? setupAction = null)
    {
        builder.Services.AddTransient(_ => cachingOptions ?? new()); // todo: use options?
        builder.Services.AddMemoryCache(setupAction ?? (_ => { }));
        builder.Services.Decorate<IAccessTokenProvider, InMemoryCachingAccessTokenProvider>();

        return builder;
    }

    //todo: what should happen if someone used both in memory and distributed caching?
    // todo: should we use .WithInMemoryCaching/.WithDistributedCaching or .WithCaching(CachingType.InMemory/CachingType.Distributed)
}

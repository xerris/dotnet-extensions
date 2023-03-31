using Microsoft.Extensions.DependencyInjection;
using Xerris.Extensions.Http.OAuth.Internal;

namespace Xerris.Extensions.Http.OAuth;

/// <summary>
/// Extension methods for configuring access token providers in an <see cref="IServiceCollection"/>.
/// </summary>
public static class AccessTokenProviderServiceCollectionExtensions
{
    public static IServiceCollection AddAccessTokenProvider(this IServiceCollection services,
        Action<IAccessTokenProviderBuilder> configure)
    {
        configure(new AccessTokenProviderBuilder(services));

        return services;
    }
}

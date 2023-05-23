using Microsoft.Extensions.DependencyInjection;
using Xerris.Extensions.Authentication.OAuth.Internal;

namespace Xerris.Extensions.Authentication.OAuth;

/// <summary>
/// Extension methods for configuring access token providers in an <see cref="IServiceCollection" />.
/// </summary>
public static class AccessTokenProviderServiceCollectionExtensions
{
    /// <summary>
    /// Adds an access token provider to the <see cref="IServiceCollection" />.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection" /> to add services to.</param>
    /// <param name="configure">The <see cref="IAccessTokenProviderBuilder" /> configuration delegate.</param>
    /// <returns>The <see cref="IServiceCollection" /> instance.</returns>
    public static IServiceCollection AddAccessTokenProvider(this IServiceCollection services,
        Action<IAccessTokenProviderBuilder> configure)
    {
        configure(new AccessTokenProviderBuilder(services));

        return services;
    }
}

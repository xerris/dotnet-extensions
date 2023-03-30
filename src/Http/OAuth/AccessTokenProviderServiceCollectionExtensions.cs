using Microsoft.Extensions.DependencyInjection;
using Xerris.Extensions.Http.OAuth.Internal;

namespace Xerris.Extensions.Http.OAuth;

public static class AccessTokenProviderServiceCollectionExtensions
{
    public static IServiceCollection AddAccessTokenProvider(this IServiceCollection services,
        Action<IAccessTokenProviderBuilder> configure)
    {
        configure(new AccessTokenProviderBuilder(services));

        return services;
    }
}

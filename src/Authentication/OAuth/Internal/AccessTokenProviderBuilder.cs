using Microsoft.Extensions.DependencyInjection;

namespace Xerris.Extensions.Authentication.OAuth.Internal;

internal sealed class AccessTokenProviderBuilder : IAccessTokenProviderBuilder
{
    public AccessTokenProviderBuilder(IServiceCollection services)
    {
        Services = services;
    }

    public IServiceCollection Services { get; }
}

using Microsoft.Extensions.DependencyInjection;

namespace Xerris.Extensions.Authentication.OAuth.Internal;

internal sealed class AccessTokenProviderBuilder(IServiceCollection services) : IAccessTokenProviderBuilder
{
    public IServiceCollection Services { get; } = services;
}

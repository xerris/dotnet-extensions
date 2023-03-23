using Microsoft.Extensions.DependencyInjection;

namespace Xerris.Extensions.Http.OAuth;

public interface IAccessTokenProviderBuilder
{
    IServiceCollection Services { get; }
}

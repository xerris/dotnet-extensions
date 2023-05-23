using Microsoft.Extensions.DependencyInjection;

namespace Xerris.Extensions.Authentication.OAuth;

/// <summary>
/// An interface for configuring access token providers.
/// </summary>
public interface IAccessTokenProviderBuilder
{
    /// <summary>
    /// The <see cref="IServiceCollection" /> where access token providers are configured.
    /// </summary>
    IServiceCollection Services { get; }
}

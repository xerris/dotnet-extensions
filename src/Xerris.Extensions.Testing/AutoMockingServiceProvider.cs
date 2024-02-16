using Microsoft.Extensions.DependencyInjection;
using Moq.AutoMock;

namespace Xerris.Extensions.Testing;

internal sealed class AutoMockingServiceProvider(IServiceProvider serviceProvider, AutoMocker mocker) : IServiceProvider
{
    public object GetService(Type serviceType)
    {
        return serviceProvider.GetService(serviceType) ?? mocker.GetRequiredService(serviceType);
    }
}

using Moq.AutoMock;

namespace Xerris.Extensions.Testing;

internal class AutoMockingServiceProvider : IServiceProvider
{
    private readonly AutoMocker _mocker;
    private readonly IServiceProvider _serviceProvider;

    public AutoMockingServiceProvider(IServiceProvider serviceProvider, AutoMocker mocker)
    {
        _serviceProvider = serviceProvider;
        _mocker = mocker;
    }

    public object GetService(Type serviceType)
    {
        return _serviceProvider.GetService(serviceType) ?? _mocker.Get(serviceType);
    }
}

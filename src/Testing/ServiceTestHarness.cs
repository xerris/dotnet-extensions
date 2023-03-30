using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.AutoMock;

namespace Xerris.Extensions.Testing;

public class ServiceTestHarness<TService>
    where TService : class
{
    private readonly IServiceCollection _serviceCollection = new ServiceCollection();

    private readonly Func<TService, Task> _testAction;

    private ServiceTestHarness(Func<TService, Task> testAction)
    {
        _testAction = testAction;
    }

    public ServiceTestHarness<TService> WithDependency<TDependency>(TDependency dependency)
        where TDependency : class
    {
        _serviceCollection.AddTransient(_ => dependency);

        return this;
    }

    public ServiceTestHarness<TService> WithDependency<TDependency>(Mock<TDependency> mock)
        where TDependency : class
    {
        return WithDependency(mock.Object);
    }

    public ServiceTestHarness<TService> WithDependency<TDependency>()
        where TDependency : class
    {
        _serviceCollection.AddTransient<TDependency>();

        return this;
    }

    public ServiceTestHarness<TService> WithServices(Action<IServiceCollection> configure)
    {
        configure(_serviceCollection);

        return this;
    }

    public async Task TestAsync()
    {
        _serviceCollection.AddLogging(logging => logging.AddConsole());

        if (_serviceCollection.All(x => x.ServiceType != typeof(TService)))
            _serviceCollection.AddTransient<TService>();

        var factory = new AutoMockingServiceProviderFactory(new AutoMocker(MockBehavior.Loose));

        var serviceProvider = factory.CreateServiceProvider(_serviceCollection);

        var serviceUnderTest = serviceProvider.GetRequiredService<TService>();

        await _testAction(serviceUnderTest);
    }

    public static ServiceTestHarness<TService> Create(Func<TService, Task> testAction)
    {
        return new ServiceTestHarness<TService>(testAction);
    }
}

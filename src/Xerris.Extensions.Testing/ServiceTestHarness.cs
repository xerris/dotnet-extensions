using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;

namespace Xerris.Extensions.Testing;

/// <summary>
/// A harness for executing tests against an instance of <typeparamref name="TService" /> and configuring its
/// dependencies.
/// </summary>
/// <typeparam name="TService">The type of the service under test.</typeparam>
public class ServiceTestHarness<TService>
    where TService : class
{
    private readonly IServiceCollection _serviceCollection = new ServiceCollection();

    private readonly Func<TService, Task> _testAction;

    private ServiceTestHarness(Func<TService, Task> testAction)
    {
        _testAction = testAction;
    }

    /// <summary>
    /// Create a new instance of <see cref="ServiceTestHarness{TService}" />.
    /// </summary>
    /// <param name="testAction">
    /// A delegate containing the test action to execute. Invoked when <see cref="TestAsync" /> is called.
    /// </param>
    /// <returns>The <see cref="ServiceTestHarness{TService}" /> instance.</returns>
#pragma warning disable CA1000 // Do not declare static members on generic types
    // This syntax is not confusing since this class uses a private constructor.
    public static ServiceTestHarness<TService> Create(Func<TService, Task> testAction)
#pragma warning restore CA1000 // Do not declare static members on generic types
    {
        return new ServiceTestHarness<TService>(testAction);
    }

    /// <summary>
    /// Configures a dependency with the underlying service provider.
    /// </summary>
    /// <typeparam name="TDependency">The dependency type.</typeparam>
    /// <param name="dependency">The dependency to add</param>
    /// <returns>The <see cref="ServiceTestHarness{TService}" /> instance.</returns>
    public ServiceTestHarness<TService> WithDependency<TDependency>(TDependency dependency)
        where TDependency : class
    {
        _serviceCollection.AddTransient(_ => dependency);

        return this;
    }

    /// <summary>
    /// Configures a mock dependency with the underlying service provider.
    /// </summary>
    /// <typeparam name="TDependency">The dependency type.</typeparam>
    /// <param name="mock">The mocked dependency to add.</param>
    /// <returns>The <see cref="ServiceTestHarness{TService}" /> instance.</returns>
    public ServiceTestHarness<TService> WithDependency<TDependency>(Mock<TDependency> mock)
        where TDependency : class
    {
        return WithDependency(mock.Object);
    }

    /// <summary>
    /// Configures a dependency of the specified type with the underlying service provider.
    /// </summary>
    /// <typeparam name="TDependency">The dependency type.</typeparam>
    /// <returns>The <see cref="ServiceTestHarness{TService}" /> instance.</returns>
    public ServiceTestHarness<TService> WithDependency<TDependency>()
        where TDependency : class
    {
        _serviceCollection.AddTransient<TDependency>();

        return this;
    }

    /// <summary>
    /// Configures the underlying <see cref="IServiceCollection" />.
    /// </summary>
    /// <remarks>
    /// Useful when using configuring services using <see cref="IServiceCollection" /> extension methods.
    /// </remarks>
    /// <param name="configure">
    /// A delegate to configure the <see cref="IServiceCollection" /> for the test service.
    /// </param>
    /// <returns>The <see cref="ServiceTestHarness{TService}" /> instance.</returns>
    public ServiceTestHarness<TService> WithServices(Action<IServiceCollection> configure)
    {
        configure(_serviceCollection);

        return this;
    }

    /// <summary>
    /// Execute the test action, automatically creating mocks for any missing dependencies.
    /// </summary>
    /// <returns>A <see cref="Task" /> that represents the test action execution.</returns>
    public async Task TestAsync()
    {
        _serviceCollection.AddLogging(logging => logging.AddConsole());

        if (_serviceCollection.All(x => x.ServiceType != typeof(TService)))
            _serviceCollection.AddTransient<TService>();

        var factory = new AutoMockingServiceProviderFactory();

        var serviceProvider = factory.CreateServiceProvider(_serviceCollection);

        var serviceUnderTest = serviceProvider.GetRequiredService<TService>();

        await _testAction(serviceUnderTest).ConfigureAwait(false);
    }
}

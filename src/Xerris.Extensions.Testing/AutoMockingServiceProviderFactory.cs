using Microsoft.Extensions.DependencyInjection;
using Moq;
using Moq.AutoMock;

namespace Xerris.Extensions.Testing;

internal sealed class AutoMockingServiceProviderFactory(MockBehavior mockBehavior = MockBehavior.Loose)
    : IServiceProviderFactory<IServiceCollection>
{
    private readonly AutoMocker _mocker = new(mockBehavior);

    public IServiceCollection CreateBuilder(IServiceCollection services)
    {
        return services;
    }

    public IServiceProvider CreateServiceProvider(IServiceCollection serviceCollection)
    {
        var implicitlyConstructedServicesDescriptors =
            serviceCollection.Where(d => d.ImplementationFactory == null);

        foreach (var descriptor in implicitlyConstructedServicesDescriptors.ToArray())
        {
            var definedServiceConstructors = descriptor.ServiceType.GetConstructors()
                .Where(c => c.GetParameters().Length != 0)
                .ToArray();

            // No defined (non-default) constructors here, move on
            if (definedServiceConstructors.Length == 0)
                continue;

            // Find the least specific non-default constructor for the service type, i.e.: the one with the fewest
            // parameters.
            var bestConstructor = definedServiceConstructors.First(ctor =>
            {
                var lowestParameterCount = definedServiceConstructors.Min(c => c.GetParameters().Length);

                return ctor.GetParameters().Length == lowestParameterCount;
            });

            var ctorParameterTypes = bestConstructor.GetParameters().Select(p => p.ParameterType);

            // Don't mock framework constructs
            var filteredCtorParameterTypes = ctorParameterTypes.Where(pt =>
                !pt.Namespace!.StartsWith("System", StringComparison.InvariantCulture) &&
                !pt.Namespace!.StartsWith("Microsoft.Extensions", StringComparison.InvariantCulture));

            foreach (var parameterType in filteredCtorParameterTypes)
            {
                // If there's already a type registered for this constructor parameter, move on, otherwise register a
                // mocked version of it.
                if (serviceCollection.Any(sd => sd.ServiceType == parameterType))
                    continue;

                var mockType = typeof(Mock<>);
                mockType = mockType.MakeGenericType(parameterType);

                var mockInstance = Activator.CreateInstance(mockType) as Mock;

                serviceCollection.Add(new ServiceDescriptor(parameterType, mockInstance!.Object));
            }
        }

        return new AutoMockingServiceProvider(serviceCollection.BuildServiceProvider(), _mocker);
    }
}

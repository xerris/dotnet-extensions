using Microsoft.Extensions.DependencyInjection;

namespace Xerris.Extensions.BackgroundTasks;

/// <summary>
/// Extension methods for configuring background task processing services with a <see cref="IServiceCollection" />.
/// </summary>
public static class BackgroundTaskServiceCollectionExtensions
{
    /// <summary>
    /// Registers background task processing services with the service collection.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection" /> to add the services to.</param>
    /// <param name="configure">
    /// A delegate to configure the <see cref="BackgroundTaskQueueOptions" /> to use.
    /// </param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    public static IServiceCollection AddBackgroundTaskProcessing(this IServiceCollection services,
        Action<BackgroundTaskQueueOptions> configure)
    {
        services.AddOptions<BackgroundTaskQueueOptions>().Configure(configure);
        services.AddSingleton<IBackgroundTaskQueue, BackgroundTaskQueue>();

        services.AddHostedService<BackgroundTaskQueueProcessor>();

        return services;
    }
}

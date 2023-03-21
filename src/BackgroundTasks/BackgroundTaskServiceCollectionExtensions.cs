using Microsoft.Extensions.DependencyInjection;

namespace Xerris.Extensions.BackgroundTasks;

public static class BackgroundTaskServiceCollectionExtensions
{
    /// <summary>
    /// Registers background task processing services with the service collection.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the services to.</param>
    /// <param name="options">The task processing options.</param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    public static IServiceCollection AddBackgroundTaskProcessing(this IServiceCollection services,
        BackgroundTaskQueueOptions options)
    {
        services.AddTransient(_ => options);
        services.AddSingleton<IBackgroundTaskQueue, BackgroundTaskQueue>();

        services.AddHostedService<BackgroundTaskQueueProcessor>();

        return services;
    }
}

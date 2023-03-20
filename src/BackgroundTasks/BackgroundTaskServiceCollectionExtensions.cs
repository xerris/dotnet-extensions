using Microsoft.Extensions.DependencyInjection;

namespace Xerris.Extensions.BackgroundTasks;

public static class BackgroundTaskServiceCollectionExtensions
{
    public static IServiceCollection AddBackgroundTaskProcessing(this IServiceCollection services,
        BackgroundTaskQueueOptions options)
    {
        services.AddTransient(_ => options);
        services.AddSingleton<IBackgroundTaskQueue, BackgroundTaskQueue>();

        services.AddHostedService<BackgroundTaskQueueProcessor>();

        return services;
    }
}

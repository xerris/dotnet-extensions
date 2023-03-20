namespace Xerris.Extensions.BackgroundTasks;

public interface IBackgroundTaskQueue
{
    ValueTask QueueBackgroundWorkItemAsync(Func<CancellationToken, ValueTask> workItem);

    ValueTask QueueBackgroundWorkItemAsync(IBackgroundTask backgroundTask);

    ValueTask<Func<CancellationToken, ValueTask>> DequeueAsync(CancellationToken cancellationToken);

    bool TryPeek(out Func<CancellationToken, ValueTask>? workItem);

    int Count { get; }
}

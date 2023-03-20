using System.Threading.Channels;

namespace Xerris.Extensions.BackgroundTasks;

/// <summary>
/// Options for configuring background job processing behaviour.
/// </summary>
/// <param name="QueueCapacity">
/// The capacity of the background task queue.
/// <remarks>
/// <paramref name="QueueCapacity"/> should be set based on the expected application load and number of concurrent
/// threads accessing the queue. <c>BoundedChannelFullMode.Wait</c> will cause calls to <c>WriteAsync()</c> to return a
/// task, which completes only when space becomes available. This can lead to back-pressure if too many
/// publishers/calls start accumulating.
/// </remarks>
/// </param>
/// <param name="WorkerCount">The number of background processing workers to use.</param>
public record BackgroundTaskQueueOptions(int QueueCapacity, int WorkerCount);

public class BackgroundTaskQueue : IBackgroundTaskQueue
{
    private readonly Channel<Func<CancellationToken, ValueTask>> _queue;

    public BackgroundTaskQueue(BackgroundTaskQueueOptions options)
    {
        _queue = Channel.CreateBounded<Func<CancellationToken, ValueTask>>(
            new BoundedChannelOptions(options.QueueCapacity) { FullMode = BoundedChannelFullMode.Wait });
    }

    public async ValueTask QueueBackgroundWorkItemAsync(Func<CancellationToken, ValueTask> workItem)
    {
        await _queue.Writer.WriteAsync(workItem);
    }

    public async ValueTask QueueBackgroundWorkItemAsync(IBackgroundTask backgroundTask)
    {
        await _queue.Writer.WriteAsync(backgroundTask.DoWorkAsync);
    }

    public async ValueTask<Func<CancellationToken, ValueTask>> DequeueAsync(
        CancellationToken cancellationToken)
    {
        var workItem = await _queue.Reader.ReadAsync(cancellationToken);

        return workItem;
    }

    public bool TryPeek(out Func<CancellationToken, ValueTask>? workItem)
    {
        return _queue.Reader.TryPeek(out workItem);
    }

    public int Count => _queue.Reader.Count;
}

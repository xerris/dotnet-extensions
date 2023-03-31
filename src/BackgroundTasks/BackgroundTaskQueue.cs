using System.Threading.Channels;

namespace Xerris.Extensions.BackgroundTasks;

/// <summary>
/// Options for configuring background job processing behaviour.
/// </summary>
/// <param name="QueueCapacity">
/// The capacity of the background task queue.
/// <remarks>
/// <paramref name="QueueCapacity"/> should be set based on the expected application load and number of concurrent
/// threads accessing the queue. <see cref="BoundedChannelFullMode"/><c>.Wait</c> will cause calls to
/// <c>WriteAsync()</c> to return a task, which completes only when space becomes available. This can lead to
/// back-pressure if too many publishers/calls start accumulating.
/// </remarks>
/// </param>
/// <param name="WorkerCount">The number of background processing workers to use.</param>
public record BackgroundTaskQueueOptions(int QueueCapacity, int WorkerCount);

/// <inheritdoc />
public class BackgroundTaskQueue : IBackgroundTaskQueue
{
    private readonly Channel<Func<CancellationToken, ValueTask>> _queue;

    /// <summary>
    /// Creates a new instance of <see cref="BackgroundTaskQueue"/>.
    /// </summary>
    /// <param name="options">Configuration options for the service.</param>
    public BackgroundTaskQueue(BackgroundTaskQueueOptions options)
    {
        _queue = Channel.CreateBounded<Func<CancellationToken, ValueTask>>(
            new BoundedChannelOptions(options.QueueCapacity) { FullMode = BoundedChannelFullMode.Wait });
    }

    /// <inheritdoc />
    public async ValueTask QueueBackgroundWorkItemAsync(Func<CancellationToken, ValueTask> workItem)
    {
        await _queue.Writer.WriteAsync(workItem).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async ValueTask QueueBackgroundWorkItemAsync(IBackgroundTask backgroundTask)
    {
        await _queue.Writer.WriteAsync(backgroundTask.DoWorkAsync).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async ValueTask<Func<CancellationToken, ValueTask>> DequeueAsync(
        CancellationToken cancellationToken)
    {
        var workItem = await _queue.Reader.ReadAsync(cancellationToken).ConfigureAwait(false);

        return workItem;
    }


    /// <inheritdoc />
    public bool TryPeek(out Func<CancellationToken, ValueTask>? workItem)
    {
        return _queue.Reader.TryPeek(out workItem);
    }

    /// <inheritdoc />
    public int Count => _queue.Reader.Count;
}

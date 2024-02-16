using System.Threading.Channels;
using Microsoft.Extensions.Options;

namespace Xerris.Extensions.BackgroundTasks;

/// <summary>
/// A background task queue that processes work items with a bounded <see cref="Channel"/>.
/// </summary>
public class BackgroundTaskQueue : IBackgroundTaskQueue
{
    private readonly Channel<Func<CancellationToken, ValueTask>> _queue;

    /// <summary>
    /// Creates a new instance of <see cref="BackgroundTaskQueue"/>.
    /// </summary>
    /// <param name="options">Configuration options for the service.</param>
    public BackgroundTaskQueue(IOptions<BackgroundTaskQueueOptions> options)
    {
        _queue = Channel.CreateBounded<Func<CancellationToken, ValueTask>>(
            new BoundedChannelOptions(options.Value.QueueCapacity) { FullMode = BoundedChannelFullMode.Wait });
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

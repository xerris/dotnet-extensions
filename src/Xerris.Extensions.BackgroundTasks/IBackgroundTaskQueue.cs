namespace Xerris.Extensions.BackgroundTasks;

/// <summary>
/// An interface for services that process background tasks.
/// </summary>
public interface IBackgroundTaskQueue
{
    /// <summary>
    /// Enqueue a function to be processed.
    /// </summary>
    /// <param name="workItem">A function representing the work to be performed.</param>
    /// <returns>The result of the operation.</returns>
    ValueTask QueueBackgroundWorkItemAsync(Func<CancellationToken, ValueTask> workItem);

    /// <summary>
    /// Enqueue a background task to be processed.
    /// </summary>
    /// <param name="backgroundTask">The background task to be processed.</param>
    /// <returns>The result of the operation.</returns>
    ValueTask QueueBackgroundWorkItemAsync(IBackgroundTask backgroundTask);

    /// <summary>
    /// Get the next task to be processed.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the work to process.</returns>
    ValueTask<Func<CancellationToken, ValueTask>> DequeueAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Attempts to peek at the next background work item in the queue.
    /// </summary>
    /// <param name="workItem">The next work item in the queue</param>
    /// <returns><c>true</c> if an item was read; otherwise, <c>false</c> if no item was read.</returns>
    bool TryPeek(out Func<CancellationToken, ValueTask>? workItem);

    /// <summary>
    /// The current number of work items in the queue.
    /// </summary>
    int Count { get; }
}

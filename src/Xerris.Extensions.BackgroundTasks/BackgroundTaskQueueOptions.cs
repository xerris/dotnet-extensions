using System.Threading.Channels;
using Microsoft.Extensions.Options;

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
public record BackgroundTaskQueueOptions(int QueueCapacity, int WorkerCount) : IOptions<BackgroundTaskQueueOptions>
{
    /// <summary>
    /// The default configured M<see cref="BackgroundTaskQueueOptions" /> instance.
    /// </summary>
    public BackgroundTaskQueueOptions Value => this;
}

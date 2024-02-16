using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Xerris.Extensions.BackgroundTasks;

/// <summary>
/// A <see cref="BackgroundService"/> that processes a queue of work items.
/// </summary>
/// <remarks>
/// Create a new instance of <see cref="BackgroundTaskQueueProcessor"/>.
/// </remarks>
/// <param name="taskQueue">The task queue to process.</param>
/// <param name="options">The task processing options.</param>
/// <param name="logger">The logger to log messages to.</param>
public class BackgroundTaskQueueProcessor(IBackgroundTaskQueue taskQueue, IOptions<BackgroundTaskQueueOptions> options,
    ILogger<BackgroundTaskQueueProcessor> logger) : BackgroundService
{
    private readonly BackgroundTaskQueueOptions _options = options.Value;

    /// <inheritdoc />
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("Background task processor is running");

        await BackgroundProcessing(stoppingToken).ConfigureAwait(false);
    }

    private async Task BackgroundProcessing(CancellationToken stoppingToken)
    {
        async Task DoWorkAsync(CancellationToken cancellationToken)
        {
            var workItem = await taskQueue.DequeueAsync(cancellationToken).ConfigureAwait(false);

            await workItem(stoppingToken).ConfigureAwait(false);
        }

        while (!stoppingToken.IsCancellationRequested)
        {
            var jobs = Enumerable.Range(0, _options.WorkerCount).Select(_ => DoWorkAsync(stoppingToken));

            await Task.WhenAll(jobs).ConfigureAwait(false);
        }
    }

    /// <inheritdoc />
    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("Background task processor is stopping");

        await base.StopAsync(cancellationToken).ConfigureAwait(false);
    }
}

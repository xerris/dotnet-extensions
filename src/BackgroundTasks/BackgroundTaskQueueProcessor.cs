using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Xerris.Extensions.BackgroundTasks;

public class BackgroundTaskQueueProcessor : BackgroundService
{
    private readonly IBackgroundTaskQueue _taskQueue;
    private readonly BackgroundTaskQueueOptions _options;
    private readonly ILogger<BackgroundTaskQueueProcessor> _logger;

    public BackgroundTaskQueueProcessor(IBackgroundTaskQueue taskQueue, BackgroundTaskQueueOptions options,
        ILogger<BackgroundTaskQueueProcessor> logger)
    {
        _taskQueue = taskQueue;
        _options = options;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Background task processor is running");

        await BackgroundProcessing(stoppingToken).ConfigureAwait(false);
    }

    private async Task BackgroundProcessing(CancellationToken stoppingToken)
    {
        async Task DoWorkAsync(CancellationToken cancellationToken)
        {
            var workItem = await _taskQueue.DequeueAsync(cancellationToken).ConfigureAwait(false);

            await workItem(stoppingToken).ConfigureAwait(false);
        }

        while (!stoppingToken.IsCancellationRequested)
        {
            var jobs = Enumerable.Range(0, _options.WorkerCount).Select(_ => DoWorkAsync(stoppingToken));

            await Task.WhenAll(jobs).ConfigureAwait(false);
        }
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Background task processor is stopping");

        await base.StopAsync(cancellationToken).ConfigureAwait(false);
    }
}

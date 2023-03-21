namespace Xerris.Extensions.BackgroundTasks;

/// <summary>
/// A task to be processed by an <see cref="IBackgroundTaskQueue"/>.
/// </summary>
public interface IBackgroundTask
{
    ValueTask DoWorkAsync(CancellationToken cancellationToken);
}

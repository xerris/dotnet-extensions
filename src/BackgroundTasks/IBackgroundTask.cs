namespace Xerris.Extensions.BackgroundTasks;

/// <summary>
/// A task to be processed by an <see cref="IBackgroundTaskQueue"/>.
/// </summary>
public interface IBackgroundTask
{
    /// <summary>
    /// Starts the work for the background task.
    /// </summary>
    /// <param name="cancellationToken">Used to cancel the background task if it should be stopped.</param>
    /// <returns>A value task representing the background job.</returns>
    ValueTask DoWorkAsync(CancellationToken cancellationToken);
}

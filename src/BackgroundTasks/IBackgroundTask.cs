namespace Xerris.Extensions.BackgroundTasks;

public interface IBackgroundTask
{
    ValueTask DoWorkAsync(CancellationToken cancellationToken);
}

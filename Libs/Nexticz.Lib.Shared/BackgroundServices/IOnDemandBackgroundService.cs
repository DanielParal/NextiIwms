namespace Nexticz.Lib.Shared.BackgroundServices;

public interface IOnDemandBackgroundService
{
    Task DoWorkAsync(CancellationToken cancellationToken); 
}
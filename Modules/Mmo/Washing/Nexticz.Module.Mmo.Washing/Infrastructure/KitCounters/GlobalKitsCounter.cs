using Marten;
using Nexticz.Module.Mmo.Washing.Application.Interfaces;
using Nexticz.Module.Mmo.Washing.Domain.BatchAggregate.Events;

namespace Nexticz.Module.Mmo.Washing.Infrastructure.KitCounters;

internal class GlobalKitsCounter : IGlobalKitsCounter, IDisposable
{
    private int _count;
    private volatile bool _isInitialized;
    private readonly SemaphoreSlim _semaphore = new(1, 1);

    public async Task InitializeAsync(IDocumentSession session, CancellationToken cancellationToken)
    {
        if (_isInitialized)
            return;

        var lastKitWashCycleFinishedEvent = await session
            .Events
            .QueryRawEventDataOnly<KitCounterIncremented>()
            .OrderByDescending(x => x.CompletedKitsCount)
            .FirstOrDefaultAsync(cancellationToken);
        
        var count = lastKitWashCycleFinishedEvent?.CompletedKitsCount ?? 0;

        await _semaphore.WaitAsync(cancellationToken);
        try
        {
            if (!_isInitialized) // Double-check to avoid race conditions
            {
                _count = count;
                _isInitialized = true;
            }
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public int GetAndIncrementCurrentValue()
    {
        // Thread safe operation to avoid race conditions
        _semaphore.Wait();
        try
        {
            _count++;
            return _count;
        }
        finally
        {
            _semaphore.Release();
        }
    }
    
    public void Dispose()
    {
        _semaphore.Dispose();
    }
}
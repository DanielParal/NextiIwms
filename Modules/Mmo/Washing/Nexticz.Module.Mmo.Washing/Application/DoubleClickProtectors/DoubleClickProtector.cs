using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.FeatureManagement;
using Nexticz.Lib.Shared.Time;
using Nexticz.Module.Mmo.Washing.Domain.BatchAggregate;

namespace Nexticz.Module.Mmo.Washing.Application.DoubleClickProtectors;

internal class DoubleClickProtector(
    IFeatureManager featureManager,
    IMemoryCache memoryCache,
    IClock clock,
    ILogger<IDoubleClickProtector> logger) 
    : IDoubleClickProtector
{
    private const string KeyPrefix = "Mmo_DoubleClickProtector_";
    private const int ProtectionPeriodInSeconds = 30;
    private const string MmoFeatureDoubleClickProtection = nameof(MmoFeatureDoubleClickProtection);
    
    public async Task<bool> CanFinishKitAsync(Batch batch, string worker)
    {
        if (!await featureManager.IsEnabledAsync(nameof(MmoFeatureDoubleClickProtection)))
            return true;

        var canFinishBatch = CanFinishBatch(batch.Id, worker);
        if (batch.SisterBatchId is null)
            return canFinishBatch;

        return canFinishBatch && CanFinishBatch(batch.SisterBatchId.Value, worker);
    }

    private bool CanFinishBatch(Guid batchId, string worker)
    {
        var cacheKey = KeyPrefix + batchId;
        
        if (!memoryCache.TryGetValue(cacheKey, out DateTimeOffset cachedTime))
        {
            SetCacheEntry(cacheKey);
            return true;
        }

        if (cachedTime.AddSeconds(ProtectionPeriodInSeconds) < clock.UtcNowOffset)
            return true;

        logger.LogWarning("Double-click detected for batch {BatchId} by worker {Worker}", batchId, worker);
        return false;
    }

    private void SetCacheEntry(string cacheKey)
    {
        var now = clock.UtcNowOffset;
        memoryCache.Set(cacheKey, now, TimeSpan.FromSeconds(ProtectionPeriodInSeconds));
    }
}
using Nexticz.Module.Mmo.Reporting.Application.Interfaces;
using Nexticz.Module.Mmo.Reporting.Domain.Views;

namespace Nexticz.Module.Mmo.Reporting.Infrastructure.LineItemViews;

internal class LineItemReadOnlyRepository(
    IReportingReadOnlyEventStoreRepository readOnlyEventStoreRepository) : ILineItemReadOnlyRepository
{
    public async Task<IReadOnlyList<LineItemView>> GetLineItemsByTimeRangeAsync(DateTimeOffset startDate, DateTimeOffset endDate, CancellationToken cancellationToken)
    {
        return await readOnlyEventStoreRepository.GetAllByConditionAsync<LineItemView>(item => 
                (item.StartDate >= startDate && item.StartDate <= endDate) || 
                (item.EndDate >= startDate && item.EndDate <= endDate), 
            cancellationToken);
    }

    public async Task<IReadOnlyList<LineItemView>> GetPlannedLineItemsByLineCodeAndTimeRangeAsync(
        string lineCode, DateTimeOffset startDate, DateTimeOffset endDate,
        CancellationToken cancellationToken)
    {
        var upperLineCode = lineCode.ToUpperInvariant();
        
        return await readOnlyEventStoreRepository.GetAllByConditionAsync<LineItemView>(item => 
                item.LineCode == upperLineCode && item.IsPlanned &&
                item.StartDate <= endDate && item.EndDate >= startDate, 
            cancellationToken);
    }

    public async Task<IReadOnlyList<LineItemView>> GetLineItemsByShiftIdAsync(Guid shiftId, CancellationToken cancellationToken)
    {
        return await readOnlyEventStoreRepository.GetAllByConditionAsync<LineItemView>(item => item.ShiftId == shiftId, cancellationToken);
    }

    public async Task<IReadOnlyList<LineItemView>> GetLineItemsByKitIdAsync(Guid kitId, CancellationToken cancellationToken)
    {
        return await readOnlyEventStoreRepository.GetAllByConditionAsync<LineItemView>(item => item.KitId == kitId, cancellationToken);
    }
}
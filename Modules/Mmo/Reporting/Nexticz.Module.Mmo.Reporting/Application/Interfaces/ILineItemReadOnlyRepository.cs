using Nexticz.Module.Mmo.Reporting.Domain.Views;

namespace Nexticz.Module.Mmo.Reporting.Application.Interfaces;

internal interface ILineItemReadOnlyRepository
{
    Task<IReadOnlyList<LineItemView>> GetLineItemsByTimeRangeAsync(DateTimeOffset startDate, DateTimeOffset endDate, CancellationToken cancellationToken);
    Task<IReadOnlyList<LineItemView>> GetPlannedLineItemsByLineCodeAndTimeRangeAsync(string lineCode, DateTimeOffset startDate, DateTimeOffset endDate, CancellationToken cancellationToken);
    Task<IReadOnlyList<LineItemView>> GetLineItemsByShiftIdAsync(Guid shiftId, CancellationToken cancellationToken);
    Task<IReadOnlyList<LineItemView>> GetLineItemsByKitIdAsync(Guid kitId, CancellationToken cancellationToken);
}
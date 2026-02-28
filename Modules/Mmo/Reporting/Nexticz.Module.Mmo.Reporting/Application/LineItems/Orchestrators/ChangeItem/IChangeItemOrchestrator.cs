using ErrorOr;
using Nexticz.Module.Mmo.Reporting.Contracts.LineItems;

namespace Nexticz.Module.Mmo.Reporting.Application.LineItems.Orchestrators.ChangeItem;

internal interface IChangeItemOrchestrator
{
    Task<ErrorOr<Success>> ChangeItemAsync(
        Guid lineItemId, DateTimeOffset cutTime, ChangeLineItemTypeContract newType, 
        Guid? inactivityReasonId, CancellationToken cancellationToken);
}
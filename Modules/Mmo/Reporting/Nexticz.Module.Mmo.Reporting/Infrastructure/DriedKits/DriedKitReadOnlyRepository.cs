using Nexticz.Module.Mmo.Reporting.Application.Interfaces;
using Nexticz.Module.Mmo.Reporting.Domain.DriedKitAggregate;

namespace Nexticz.Module.Mmo.Reporting.Infrastructure.DriedKits;

internal class DriedKitReadOnlyRepository(IReportingReadOnlyEventStoreRepository reportingReadOnlyEventStoreRepository) : IDriedKitReadOnlyRepository
{
    public async Task<DriedKit?> GetDriedKitByIdFromDryingAsync(Guid kitIdFromDrying, CancellationToken cancellationToken)
    {
        return await reportingReadOnlyEventStoreRepository.GetFirstByConditionAsync<DriedKit>(x => x.KitIdFromDrying == kitIdFromDrying, cancellationToken);
    }
}
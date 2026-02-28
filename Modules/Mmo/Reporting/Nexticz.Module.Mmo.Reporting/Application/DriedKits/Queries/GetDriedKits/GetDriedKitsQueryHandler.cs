using MediatR;
using Nexticz.Lib.Shared.DevExtreme;
using Nexticz.Module.Mmo.Reporting.Application.Interfaces;
using Nexticz.Module.Mmo.Reporting.Domain.DriedKitAggregate;


namespace Nexticz.Module.Mmo.Reporting.Application.DriedKits.Queries.GetDriedKits;

internal class GetDriedKitsQueryHandler(IReportingReadOnlyEventStoreRepository reportingReadOnlyRepository) 
    : IRequestHandler<GetDriedKitsQuery, FilteredResult<DriedKit>>
{
    public async Task<FilteredResult<DriedKit>> Handle(GetDriedKitsQuery request, CancellationToken cancellationToken)
    {
        return await reportingReadOnlyRepository.GetFilteredAsync<DriedKit>(request.FilteringParams, cancellationToken);
    }
}
using MediatR;
using Nexticz.Lib.Shared.DevExtreme;
using Nexticz.Module.Sign.Settings.Application.Interfaces;
using Nexticz.Module.Sign.Settings.Domain.PartnerAggregate;


namespace Nexticz.Module.Sign.Settings.Application.Partners.Queries.GetPartners;

internal class GetPartnersQueryHandler
    (ISettingsReadOnlyEventStoreRepository readOnlyEventStoreRepository) 
    : IRequestHandler<GetPartnersQuery, FilteredResult<Partner>>
{
    public async Task<FilteredResult<Partner>> Handle(GetPartnersQuery request, CancellationToken cancellationToken)
    {
        return await readOnlyEventStoreRepository.GetFilteredAsync<Partner>(request.FilteringParams, cancellationToken);
    }
}
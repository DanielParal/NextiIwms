using MediatR;
using Nexticz.Lib.Shared.DevExtreme;
using Nexticz.Module.Sign.Settings.Application.Interfaces;
using Nexticz.Module.Sign.Settings.Domain.DeliveryMethodAggregate;


namespace Nexticz.Module.Sign.Settings.Application.DeliveryMethods.Queries.GetDeliveryMethods;

internal class GetDeliveryMethodsQueryHandler (ISettingsReadOnlyEventStoreRepository readOnlyEventStoreRepository) 
    : IRequestHandler<GetDeliveryMethodsQuery, FilteredResult<DeliveryMethod>>
{
    public async Task<FilteredResult<DeliveryMethod>> Handle(GetDeliveryMethodsQuery request, CancellationToken cancellationToken)
    {
        return await readOnlyEventStoreRepository.GetFilteredAsync<DeliveryMethod>(request.FilteringParams, cancellationToken);
    }
}
using ErrorOr;
using MediatR;
using Nexticz.Module.Sign.Settings.Application.Interfaces;
using Nexticz.Module.Sign.Settings.Domain.DeliveryMethodAggregate;

namespace Nexticz.Module.Sign.Settings.Application.DeliveryMethods.Queries.GetDeliveryMethodByCode;

internal class GetDeliveryMethodByCodeQueryHandler(ISettingsReadOnlyEventStoreRepository readOnlyEventStoreRepository) 
    : IRequestHandler<GetDeliveryMethodByCodeQuery, ErrorOr<DeliveryMethod>>
{
    public async Task<ErrorOr<DeliveryMethod>> Handle(GetDeliveryMethodByCodeQuery request, CancellationToken cancellationToken)
    {
        var deliveryMethod = await readOnlyEventStoreRepository
            .GetFirstByConditionAsync<DeliveryMethod>(
                x => x.Code.Equals(request.Code, StringComparison.InvariantCultureIgnoreCase), 
                cancellationToken);

        if (deliveryMethod is null)
            return DeliveryMethodErrors.CodeDoesNotExist;
        
        return deliveryMethod;
    }
}
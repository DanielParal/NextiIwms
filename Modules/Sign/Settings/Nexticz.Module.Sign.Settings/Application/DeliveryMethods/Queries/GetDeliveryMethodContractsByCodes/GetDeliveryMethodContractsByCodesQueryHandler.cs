using Marten;
using MediatR;
using Nexticz.Module.Sign.Settings.Contracts.DeliveryMethods;
using Nexticz.Module.Sign.Settings.Contracts.DeliveryMethods.Queries;
using Nexticz.Module.Sign.Settings.Application.Interfaces;
using Nexticz.Module.Sign.Settings.Domain.DeliveryMethodAggregate;

namespace Nexticz.Module.Sign.Settings.Application.DeliveryMethods.Queries.GetDeliveryMethodContractsByCodes;

internal class GetDeliveryMethodContractsByCodesQueryHandler(
    ISettingsReadOnlyEventStoreRepository readOnlyEventStoreRepository) : IRequestHandler<GetDeliveryMethodContractsByCodesQuery, DeliveryMethodContract[]>
{
    public async Task<DeliveryMethodContract[]> Handle(GetDeliveryMethodContractsByCodesQuery request, CancellationToken cancellationToken)
    {
        var upperCodes = request.Codes.Select(x => x.ToUpperInvariant()).ToArray();
        var deliveryMethods = await readOnlyEventStoreRepository
            .GetAllByConditionAsync<DeliveryMethod>(
                x => x.Code.IsOneOf(upperCodes), 
                cancellationToken);

        return deliveryMethods.Select(DeliveryMethodContractFactory.Create).ToArray();
    }
}
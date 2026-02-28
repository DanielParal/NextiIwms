using Marten;
using MediatR;
using Nexticz.Module.Sign.Settings.Contracts.Receivers;
using Nexticz.Module.Sign.Settings.Contracts.Receivers.Queries;
using Nexticz.Module.Sign.Settings.Application.Interfaces;
using Nexticz.Module.Sign.Settings.Domain.ReceiverAggregate;

namespace Nexticz.Module.Sign.Settings.Application.Receivers.Queries.GetReceiverContractsByCodes;

internal class GetReceiverContractsByCodesQueryHandler(
    ISettingsReadOnlyEventStoreRepository readOnlyEventStoreRepository) : IRequestHandler<GetReceiverContractsByCodesQuery, ReceiverContract[]>
{
    public async Task<ReceiverContract[]> Handle(GetReceiverContractsByCodesQuery request, CancellationToken cancellationToken)
    {
        var receiverCodes = request.ReceiverCodeWithPartnerCodes.Select(r => r.ReceiverCode).ToArray();

        var receivers = await readOnlyEventStoreRepository
            .GetAllByConditionAsync<Receiver>(
                x => x.Code.IsOneOf(receiverCodes),
                cancellationToken);
        
        var receiverPairs = request.ReceiverCodeWithPartnerCodes.ToHashSet();

        var filtered = receivers
            .Where(r => receiverPairs.Contains((r.Code, r.PartnerCode)))
            .ToArray();

        return filtered.Select(ReceiverContractFactory.Create).ToArray();
    }
}
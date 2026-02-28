using MediatR;
using Nexticz.Module.Sign.Settings.Contracts.Receivers;
using Nexticz.Module.Sign.Settings.Contracts.Receivers.Queries;
using Nexticz.Module.Sign.Settings.Application.Interfaces;
using Nexticz.Module.Sign.Settings.Domain.ReceiverAggregate;

namespace Nexticz.Module.Sign.Settings.Application.Receivers.Queries.GetReceiverContractsByName;

internal class GetReceiverContractsByNameQueryHandler(
    ISettingsReadOnlyEventStoreRepository readOnlyEventStoreRepository) : IRequestHandler<GetReceiverContractsByNameQuery, ReceiverContract[]>
{
    public async Task<ReceiverContract[]> Handle(GetReceiverContractsByNameQuery request, CancellationToken cancellationToken)
    {
        var receivers = 
            await readOnlyEventStoreRepository.GetAllByConditionAsync<Receiver>(
                x => x.Name.Contains(request.Name, StringComparison.OrdinalIgnoreCase), cancellationToken);
        
        return receivers.Select(ReceiverContractFactory.Create).ToArray();
    }
}
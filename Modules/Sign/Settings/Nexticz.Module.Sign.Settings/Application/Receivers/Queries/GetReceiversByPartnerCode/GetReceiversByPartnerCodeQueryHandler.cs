using MediatR;
using Nexticz.Module.Sign.Settings.Application.Interfaces;
using Nexticz.Module.Sign.Settings.Domain.ReceiverAggregate;

namespace Nexticz.Module.Sign.Settings.Application.Receivers.Queries.GetReceiversByPartnerCode;

internal class GetReceiversByPartnerCodeQueryHandler(
    ISettingsReadOnlyEventStoreRepository readOnlyEventStoreRepository) 
    : IRequestHandler<GetReceiversByPartnerCodeQuery, Receiver[]>
{
    public async Task<Receiver[]> Handle(GetReceiversByPartnerCodeQuery request, CancellationToken cancellationToken)
    {
        var upperCode = request.PartnerCode.ToUpperInvariant();
        var receivers = await readOnlyEventStoreRepository
            .GetAllByConditionAsync<Receiver>(
                x => x.PartnerCode.Equals(upperCode), 
                cancellationToken);
        
        return receivers.ToArray();
    }
}
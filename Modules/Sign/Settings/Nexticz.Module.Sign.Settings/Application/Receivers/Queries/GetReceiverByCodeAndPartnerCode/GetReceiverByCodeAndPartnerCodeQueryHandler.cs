using ErrorOr;
using MediatR;
using Nexticz.Module.Sign.Settings.Application.Interfaces;
using Nexticz.Module.Sign.Settings.Domain.ReceiverAggregate;

namespace Nexticz.Module.Sign.Settings.Application.Receivers.Queries.GetReceiverByCodeAndPartnerCode;

internal class GetReceiverByCodeAndPartnerCodeQueryHandler(
    ISettingsReadOnlyEventStoreRepository readOnlyEventStoreRepository) 
    : IRequestHandler<GetReceiverByCodeAndPartnerCodeQuery, ErrorOr<Receiver>>
{
    public async Task<ErrorOr<Receiver>> Handle(GetReceiverByCodeAndPartnerCodeQuery request, CancellationToken cancellationToken)
    {
        var receiver = await readOnlyEventStoreRepository
            .GetFirstByConditionAsync<Receiver>(
                x => x.Code.Equals(request.Code, StringComparison.InvariantCultureIgnoreCase) 
                     && x.PartnerCode.Equals(request.PartnerCode, StringComparison.InvariantCultureIgnoreCase), 
                cancellationToken);

        if (receiver is null)
            return ReceiverErrors.CombinationCodeAndPartnerCodeNotFound;
        
        return receiver;
    }
}
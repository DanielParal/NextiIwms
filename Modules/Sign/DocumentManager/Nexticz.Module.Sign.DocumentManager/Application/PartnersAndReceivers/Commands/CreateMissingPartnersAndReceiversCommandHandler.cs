using ErrorOr;
using MediatR;
using Microsoft.Extensions.Logging;
using Nexticz.Module.Sign.DocumentManager.Contracts.PartnersAndReceivers;
using Nexticz.Module.Sign.Settings.Contracts.Partners.Queries;
using Nexticz.Module.Sign.Settings.Contracts.Receivers.Queries;

namespace Nexticz.Module.Sign.DocumentManager.Application.PartnersAndReceivers.Commands;

internal class CreateMissingPartnersAndReceiversCommandHandler(
    IMediator mediator,
    ILogger<CreateMissingPartnersAndReceiversCommandHandler> logger) : IRequestHandler<CreateMissingPartnersAndReceiversCommand, Success>
{
    public async Task<Success> Handle(CreateMissingPartnersAndReceiversCommand request, CancellationToken cancellationToken)
    {
        
        var receiverCodeWithPartnerCodeTuples =
            request.DeliveryDocuments.Select(x => (x.OperationalUnitCode, x.PartnerCode))
                .Distinct()
                .ToArray();
        
        var receiverContracts =
            await mediator.Send(new GetReceiverContractsByCodesQuery(receiverCodeWithPartnerCodeTuples),
                cancellationToken);

        foreach (var deliveryDocument in request.DeliveryDocuments)
        {
            var existingReceiverContract =
                receiverContracts.FirstOrDefault(x => x.Code == deliveryDocument.OperationalUnitCode);
            if (existingReceiverContract != null)
                continue;

            var existingPartner =
                await mediator.Send(new GetPartnerContractByCodeQuery(deliveryDocument.PartnerCode),
                    cancellationToken);

            if (existingPartner.IsError)
                await mediator.Publish(new MissingPartnerDetectedNotification(deliveryDocument.PartnerCode, deliveryDocument.PartnerNameShort), cancellationToken);

            await mediator.Publish(
                new MissingReceiverDetectedNotification(deliveryDocument.OperationalUnitCode, deliveryDocument.PartnerCode, deliveryDocument.OperationalUnitName), 
                cancellationToken);
        }
        
        return Result.Success;
    }
}
using ErrorOr;
using MediatR;
using Microsoft.Extensions.Logging;
using Nexticz.Lib.Shared.Errors;
using Nexticz.Module.Sign.Settings.Application.Interfaces;
using Nexticz.Module.Sign.Settings.Application.Receivers.Queries.GetReceiverByCodeAndPartnerCode;
using Nexticz.Module.Sign.Settings.Domain.ReceiverAggregate;
using Nexticz.Module.Sign.Settings.Domain.ReceiverAggregate.Events;

namespace Nexticz.Module.Sign.Settings.Application.Receivers.Commands.CreateReceiver;

internal class CreateReceiverCommandHandler(
        ILogger<CreateReceiverCommandHandler> logger,
        ISettingsUnitOfWork unitOfWork,
        ISender sender
    ) : IRequestHandler<CreateReceiverCommand, ErrorOr<Receiver>>
{
    public async Task<ErrorOr<Receiver>> Handle(CreateReceiverCommand request, CancellationToken cancellationToken)
    {
        var existingReceiver = await sender.Send(new GetReceiverByCodeAndPartnerCodeQuery(request.Code, request.PartnerCode), cancellationToken);

        if (existingReceiver.HasValue())
        {
            logger.LogInformation("Sign - Object {ObjectName} with code: {Code} and partner code: {PartnerCode} already exists. Nothing to create.",
                nameof(Receiver), existingReceiver.Value.Code, existingReceiver.Value.PartnerCode);
            return ReceiverErrors.ValidationCombinationCodeAndPartnerCodeAlreadyExists;
        }
        
        var validationResult = await ReceiverValidator.ValidateAsync(request.PartnerCode, sender, logger, cancellationToken);  
        if (validationResult.IsError)
            return validationResult.Errors;
        
        var receiver = new Receiver(request.Code, validationResult.Value.PartnerCode, request.Name);
        var receiverCreatedEvent =
            new ReceiverCreatedEvent(receiver.Id, receiver.Code, receiver.PartnerCode, receiver.Name);

        unitOfWork.StartStream<ReceiverCreatedEvent, Receiver>(receiver.Id, receiverCreatedEvent);
            
        logger.LogInformation("Sign - Object {ObjectName} with code: {Code} and partner code: {PartnerCode} created.",
            nameof(Receiver), receiver.Code, receiver.PartnerCode);
        return receiver;
    }
}
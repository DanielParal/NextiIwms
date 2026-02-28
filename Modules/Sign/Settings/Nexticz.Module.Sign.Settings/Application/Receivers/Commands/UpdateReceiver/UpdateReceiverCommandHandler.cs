using ErrorOr;
using MediatR;
using Microsoft.Extensions.Logging;
using Nexticz.Lib.Shared.Errors;
using Nexticz.Module.Sign.Settings.Application.Interfaces;
using Nexticz.Module.Sign.Settings.Application.Receivers.Queries.GetReceiverByCodeAndPartnerCode;
using Nexticz.Module.Sign.Settings.Domain.ReceiverAggregate;
using Nexticz.Module.Sign.Settings.Domain.ReceiverAggregate.Events;

namespace Nexticz.Module.Sign.Settings.Application.Receivers.Commands.UpdateReceiver;

internal class UpdateReceiverCommandHandler(
    ILogger<UpdateReceiverCommandHandler> logger,
    ISettingsUnitOfWork unitOfWork,
    ISender sender) 
    : IRequestHandler<UpdateReceiverCommand, ErrorOr<Updated>>
{
    public async Task<ErrorOr<Updated>> Handle(UpdateReceiverCommand request, CancellationToken cancellationToken)
    {
        var receiver = await sender.Send(new GetReceiverByCodeAndPartnerCodeQuery(request.Code, request.PartnerCode), cancellationToken);

        if (!receiver.HasValue())
        {
            logger.LogWarning("Sign - Did not find object {ObjectName} with code: {Code} and partner code: {PartnerCode}. Nothing to update", 
                nameof(Receiver), request.Code, request.PartnerCode);
            return ReceiverErrors.ValidationCombinationCodeAndPartnerCodeDoesNotExist;
        }
        
        var validationResult = await ReceiverValidator.ValidateAsync(request.PartnerCode, sender, logger, cancellationToken);  
        if (validationResult.IsError)
            return validationResult.Errors;
        
        var receiverUpdatedEvent = new ReceiverUpdatedEvent(receiver.Value.Id, receiver.Value.Code, validationResult.Value.PartnerCode, request.Name);
        unitOfWork.AppendEvent(receiver.Value.Id, receiverUpdatedEvent);
        
        logger.LogInformation("Sign - Object {ObjectName} with code: {Code}, partner code: {PartnerCode}, name: {Name} updated.",
            nameof(Receiver), receiver.Value.Code, validationResult.Value.PartnerCode, request.Name);
        return Result.Updated;
    }
}
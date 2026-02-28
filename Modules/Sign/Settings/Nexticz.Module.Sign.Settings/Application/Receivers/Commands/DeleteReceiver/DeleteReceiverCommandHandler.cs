using ErrorOr;
using MediatR;
using Microsoft.Extensions.Logging;
using Nexticz.Module.Sign.Settings.Application.Interfaces;
using Nexticz.Module.Sign.Settings.Application.Receivers.Queries.GetReceiverByCodeAndPartnerCode;
using Nexticz.Module.Sign.Settings.Domain.ReceiverAggregate;
using Nexticz.Module.Sign.Settings.Domain.ReceiverAggregate.Events;

namespace Nexticz.Module.Sign.Settings.Application.Receivers.Commands.DeleteReceiver;

internal class DeleteReceiverCommandHandler(
    ILogger<DeleteReceiverCommandHandler> logger,
    ISender sender,
    ISettingsUnitOfWork unitOfWork
) 
    : IRequestHandler<DeleteReceiverCommand, ErrorOr<Deleted>>
{
    public async Task<ErrorOr<Deleted>> Handle(DeleteReceiverCommand request, CancellationToken cancellationToken)
    {
        var receiver = await sender.Send(new GetReceiverByCodeAndPartnerCodeQuery(request.Code, request.PartnerCode), cancellationToken);

        if (receiver.IsError)
        {
            logger.LogInformation("Sign - Did not find object {ObjectName} with code: {Code} and partner code: {PartnerCode}. Nothing to delete.", 
                nameof(Receiver), request.Code, request.PartnerCode);
            return ReceiverErrors.ValidationCombinationCodeAndPartnerCodeDoesNotExist;
        }
        
        var receiverDeletedEvent = new ReceiverDeletedEvent(receiver.Value.Id, receiver.Value.Code, receiver.Value.PartnerCode);
        unitOfWork.AppendEvent(receiver.Value.Id, receiverDeletedEvent);
        
        logger.LogInformation("Sign - Object {ObjectName} with code: {Code} and partner code: {PartnerCode} deleted.",
            nameof(Receiver), receiver.Value.Code, receiver.Value.PartnerCode);
        return Result.Deleted;
    }
}
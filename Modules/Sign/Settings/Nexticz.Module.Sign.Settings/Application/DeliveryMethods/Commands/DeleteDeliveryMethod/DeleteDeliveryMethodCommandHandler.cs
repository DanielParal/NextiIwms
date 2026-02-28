using ErrorOr;
using MediatR;
using Microsoft.Extensions.Logging;
using Nexticz.Module.Sign.Settings.Application.DeliveryMethods.Queries.GetDeliveryMethodByCode;
using Nexticz.Module.Sign.Settings.Application.Interfaces;
using Nexticz.Module.Sign.Settings.Domain.DeliveryMethodAggregate;
using Nexticz.Module.Sign.Settings.Domain.DeliveryMethodAggregate.Events;

namespace Nexticz.Module.Sign.Settings.Application.DeliveryMethods.Commands.DeleteDeliveryMethod;

internal class DeleteDeliveryMethodCommandHandler(
    ILogger<DeleteDeliveryMethodCommandHandler> logger,
    ISender sender,
    ISettingsUnitOfWork unitOfWork
) 
    : IRequestHandler<DeleteDeliveryMethodCommand, ErrorOr<Deleted>>
{
    public async Task<ErrorOr<Deleted>> Handle(DeleteDeliveryMethodCommand request, CancellationToken cancellationToken)
    {
        var deliveryMethod = await sender.Send(new GetDeliveryMethodByCodeQuery(request.Code), cancellationToken);

        if (deliveryMethod.IsError)
        {
            logger.LogInformation("Sign - Did not find object {ObjectName} with code: {Code}. Nothing to delete.", 
                nameof(DeliveryMethod), request.Code);
            return DeliveryMethodErrors.ValidationCodeDoesNotExist;
        }
        
        var deliveryMethodDeletedEvent = new DeliveryMethodDeletedEvent(deliveryMethod.Value.Id, request.Code);
        unitOfWork.AppendEvent(deliveryMethod.Value.Id, deliveryMethodDeletedEvent);
        
        logger.LogInformation("Sign - Object {ObjectName} with code: {Code} deleted.",
            nameof(DeliveryMethod), request.Code);
        return Result.Deleted;
    }
}
using ErrorOr;
using MediatR;
using Microsoft.Extensions.Logging;
using Nexticz.Module.Sign.Settings.Application.DeliveryMethods.Queries.GetDeliveryMethodByCode;
using Nexticz.Module.Sign.Settings.Application.Interfaces;
using Nexticz.Module.Sign.Settings.Domain.DeliveryMethodAggregate;
using Nexticz.Module.Sign.Settings.Domain.DeliveryMethodAggregate.Events;

namespace Nexticz.Module.Sign.Settings.Application.DeliveryMethods.Commands.UpdateDeliveryMethod;

internal class UpdateDeliveryMethodCommandHandler(
    ILogger<UpdateDeliveryMethodCommandHandler> logger,
    ISettingsUnitOfWork unitOfWork,
    ISender sender) 
    : IRequestHandler<UpdateDeliveryMethodCommand, ErrorOr<Updated>>
{
    public async Task<ErrorOr<Updated>> Handle(UpdateDeliveryMethodCommand request, CancellationToken cancellationToken)
    {
        var deliveryMethod = await sender.Send(new GetDeliveryMethodByCodeQuery(request.Code), cancellationToken);

        if (deliveryMethod.IsError)
        {
            logger.LogWarning("Sign - Settings  - Did not find object {ObjectName} with code: {Code}. Nothing to update", 
                nameof(DeliveryMethod), request.Code);
            return DeliveryMethodErrors.ValidationCodeDoesNotExist;
        }
        
        var deliveryMethodUpdatedEvent = new DeliveryMethodUpdatedEvent(
            deliveryMethod.Value.Id, deliveryMethod.Value.Code, request.Name, request.LoadingDocumentPrintCopiesCount, request.DeliveryDocumentPrintCopiesCount);
        unitOfWork.AppendEvent(deliveryMethod.Value.Id, deliveryMethodUpdatedEvent);
        
        logger.LogInformation("Sign - Settings  - Object {ObjectName} with code: {Code}, name: {Name}, " +
                              "LoadingDocumentPrintCopiesCount: {LoadingDocumentPrintCopiesCount}, DeliveryDocumentPrintCopiesCount: {DeliveryDocumentPrintCopiesCount} updated.",
            nameof(DeliveryMethod), request.Code, request.Name, request.LoadingDocumentPrintCopiesCount, request.DeliveryDocumentPrintCopiesCount);
        return Result.Updated;
    }
}
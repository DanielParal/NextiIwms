using MediatR;
using ErrorOr;
using Microsoft.Extensions.Logging;
using Nexticz.Module.Sign.SharedKernel;
using Nexticz.Lib.Shared.Errors;
using Nexticz.Module.Sign.Settings.Application.DeliveryMethods.Queries.GetDeliveryMethodByCode;
using Nexticz.Module.Sign.Settings.Application.Interfaces;
using Nexticz.Module.Sign.Settings.Domain.DeliveryMethodAggregate;
using Nexticz.Module.Sign.Settings.Domain.DeliveryMethodAggregate.Events;

namespace Nexticz.Module.Sign.Settings.Application.DeliveryMethods.Commands.CreateDeliveryMethod;

internal class CreateDeliveryMethodCommandHandler(
        ILogger<CreateDeliveryMethodCommandHandler> logger,
        ISettingsUnitOfWork unitOfWork,
        ISender sender
    ) : IRequestHandler<CreateDeliveryMethodCommand, ErrorOr<DeliveryMethod>>
{
    public async Task<ErrorOr<DeliveryMethod>> Handle(CreateDeliveryMethodCommand request, CancellationToken cancellationToken)
    {
        var existingDeliveryMethod = await sender.Send(new GetDeliveryMethodByCodeQuery(request.Code), cancellationToken);

        if (existingDeliveryMethod.HasValue())
        {
            logger.LogInformation("Sign - Settings - Object {ObjectName} with code: {Code} already exists. Nothing to create.",
                nameof(DeliveryMethod), request.Code);
            return DeliveryMethodErrors.ValidationCodeAlreadyExists;
        }
            
        var deliveryMethod = new DeliveryMethod(request.Code, request.Name, request.LoadingDocumentPrintCopiesCount, request.DeliveryDocumentPrintCopiesCount);
        var deliveryMethodCreatedEvent =
            new DeliveryMethodCreatedEvent(deliveryMethod.Id, deliveryMethod.Code, deliveryMethod.Name, deliveryMethod.LoadingDocumentPrintCopiesCount, request.DeliveryDocumentPrintCopiesCount);

        unitOfWork.StartStream<DeliveryMethodCreatedEvent, DeliveryMethod>(deliveryMethod.Id, deliveryMethodCreatedEvent);
            
        logger.LogInformation("Sign - Settings  - Object {ObjectName} with code: {Code} created.",
            nameof(DeliveryMethod), request.Code);
        return deliveryMethod;
    }
}
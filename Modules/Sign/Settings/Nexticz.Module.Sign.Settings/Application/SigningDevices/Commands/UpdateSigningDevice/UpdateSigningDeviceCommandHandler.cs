using ErrorOr;
using MediatR;
using Microsoft.Extensions.Logging;
using Nexticz.Module.Sign.Settings.Application.Locations.Queries.GetLocationByCode;
using Nexticz.Module.Sign.Settings.Application.Printers.Queries.GetPrinterByCode;
using Nexticz.Module.Sign.Settings.Contracts.SigningDevices.Notifications;
using Nexticz.Module.Sign.Settings.Application.Interfaces;
using Nexticz.Module.Sign.Settings.Application.NotificationCollectors;
using Nexticz.Module.Sign.Settings.Application.SigningDevices.Queries.GetSigningDeviceByCode;
using Nexticz.Module.Sign.Settings.Domain.SigningDeviceAggregate;
using Nexticz.Module.Sign.Settings.Domain.SigningDeviceAggregate.Events;

namespace Nexticz.Module.Sign.Settings.Application.SigningDevices.Commands.UpdateSigningDevice;

internal class UpdateSigningDeviceCommandHandler(
    ILogger<UpdateSigningDeviceCommandHandler> logger,
    ISettingsUnitOfWork unitOfWork,
    ISender sender,
    ISettingsNotificationCollector notificationCollector) 
    : IRequestHandler<UpdateSigningDeviceCommand, ErrorOr<Updated>>
{
    public async Task<ErrorOr<Updated>> Handle(UpdateSigningDeviceCommand request, CancellationToken cancellationToken)
    {
        var signingDevice = await sender.Send(new GetSigningDeviceByCodeQuery(request.Code), cancellationToken);

        if (signingDevice.IsError)
        {
            logger.LogWarning("Sign - Did not find object {ObjectName} with code: {Code}. Nothing to update", 
                nameof(SigningDevice), request.Code);
            return SigningDeviceErrors.ValidationCodeDoesNotExist;
        }
        
        var validationResult = await SigningDeviceValidator.ValidateAsync(request.LocationCode, request.PrinterCode, sender, logger, cancellationToken);  
        if (validationResult.IsError)
            return validationResult.Errors;
        
        var signingDeviceUpdatedEvent = new SigningDeviceUpdatedEvent(
            signingDevice.Value.Id, request.Code, request.Name, request.IsActive, 
            validationResult.Value.LocationCode, validationResult.Value.PrinterCode);
        unitOfWork.AppendEvent(signingDevice.Value.Id, signingDeviceUpdatedEvent);
        
        notificationCollector.AddNotification(new SigningDeviceUpdatedNotification(signingDevice.Value.Code, request.Name, request.IsActive, request.PrinterCode));
        
        logger.LogInformation("Sign - Object {ObjectName} with code: {Code}, name: {Name}, isActive: {IsActive}, printer code: {PrinterCode}, location code: {LocationCode} updated.",
            nameof(SigningDevice), request.Code, request.Name, request.IsActive, request.PrinterCode, request.LocationCode);
        return Result.Updated;
    }
}
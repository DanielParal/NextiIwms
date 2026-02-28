using ErrorOr;
using MediatR;
using Microsoft.Extensions.Logging;
using Nexticz.Module.Sign.Settings.Contracts.SigningDevices.Notifications;
using Nexticz.Lib.Shared.Errors;
using Nexticz.Module.Sign.Settings.Application.Interfaces;
using Nexticz.Module.Sign.Settings.Application.NotificationCollectors;
using Nexticz.Module.Sign.Settings.Application.SigningDevices.Queries.GetSigningDeviceByCode;
using Nexticz.Module.Sign.Settings.Domain.SigningDeviceAggregate;
using Nexticz.Module.Sign.Settings.Domain.SigningDeviceAggregate.Events;

namespace Nexticz.Module.Sign.Settings.Application.SigningDevices.Commands.CreateSigningDevice;

internal class CreateSigningDeviceCommandHandler(
        ILogger<CreateSigningDeviceCommandHandler> logger,
        ISettingsUnitOfWork unitOfWork,
        ISender sender,
        ISettingsNotificationCollector notificationCollector
    ) : IRequestHandler<CreateSigningDeviceCommand, ErrorOr<SigningDevice>>
{
    public async Task<ErrorOr<SigningDevice>> Handle(CreateSigningDeviceCommand request, CancellationToken cancellationToken)
    {
        var existingSigningDevice = await sender.Send(new GetSigningDeviceByCodeQuery(request.Code), cancellationToken);

        if (existingSigningDevice.HasValue())
        {
            logger.LogInformation("Sign - Object {ObjectName} with code: {Code} already exists. Nothing to create.",
                nameof(SigningDevice), request.Code);
            return SigningDeviceErrors.ValidationCodeAlreadyExists;
        }
        
        var validationResult = await SigningDeviceValidator.ValidateAsync(request.LocationCode, request.PrinterCode, sender, logger, cancellationToken);  
        if (validationResult.IsError)
            return validationResult.Errors;
        
        var signingDevice = new SigningDevice(request.Code, request.Name, request.IsActive, 
            validationResult.Value.LocationCode, validationResult.Value.PrinterCode);
        var signingDeviceCreatedEvent =
            new SigningDeviceCreatedEvent(signingDevice.Id, signingDevice.Code, signingDevice.Name, 
                signingDevice.IsActive, signingDevice.LocationCode, signingDevice.PrinterCode);

        unitOfWork.StartStream<SigningDeviceCreatedEvent, SigningDevice>(signingDevice.Id, signingDeviceCreatedEvent);
            
        notificationCollector.AddNotification(new SigningDeviceCreatedNotification(signingDevice.Code, signingDevice.Name, signingDevice.IsActive, signingDevice.PrinterCode));
        
        logger.LogInformation("Sign - Object {ObjectName} with code: {Code} created.",
            nameof(SigningDevice), request.Code);
        return signingDevice;
    }
}
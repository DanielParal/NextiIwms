using MediatR;
using ErrorOr;
using Microsoft.Extensions.Logging;
using Nexticz.Lib.Shared.Errors;
using Nexticz.Module.Sign.DocumentManager.Application.Interfaces;
using Nexticz.Module.Sign.DocumentManager.Application.SigningDevices.Queries.GetSigningDeviceByCode;
using Nexticz.Module.Sign.DocumentManager.Domain.SigningDeviceAggregate;
using Nexticz.Module.Sign.DocumentManager.Domain.SigningDeviceAggregate.Events;

namespace Nexticz.Module.Sign.DocumentManager.Application.SigningDevices.Commands.CreateSigningDevice;

internal class CreateSigningDeviceCommandHandler(
    ILogger<CreateSigningDeviceCommandHandler> logger,
    IDocumentManagerUnitOfWork unitOfWork,
    ISender sender) : IRequestHandler<CreateSigningDeviceCommand, ErrorOr<SigningDevice>>
{
    public async Task<ErrorOr<SigningDevice>> Handle(CreateSigningDeviceCommand request, CancellationToken cancellationToken)
    {
        var existingSigningDevice = await sender.Send(new GetSigningDeviceByCodeQuery(request.Code), cancellationToken);

        if (existingSigningDevice.HasValue())
        {
            logger.LogWarning("SIGN - DocumentManager - object {ObjectName} with code: {SigningDeviceCode} already exists.", 
                nameof(SigningDevice), existingSigningDevice.Value.Code);
            return SigningDeviceErrors.ValidationSigningDeviceAlreadyExist;       
        }
        
        var signingDevice = new SigningDevice(request.Code, request.Name, request.IsActive, request.PrinterCode, []);
        
        var signingDeviceCreatedEvent =
            new SigningDeviceCreatedEvent(
                signingDevice.Id, signingDevice.Code, signingDevice.Name, signingDevice.IsActive,  signingDevice.PrinterCode);

        unitOfWork.StartStream<SigningDeviceCreatedEvent, SigningDevice>(
            signingDevice.Id, signingDeviceCreatedEvent);
            
        logger.LogInformation("SIGN - DocumentManager - object {ObjectName} with Id: {SigningDeviceId} created, code: {SigningDeviceCode}, is active: {SigningDeviceIsActive}, printer code: {SigningDevicePrinterCode}.", 
            nameof(SigningDevice), signingDevice.Id, signingDevice.Code, signingDevice.IsActive, signingDevice.PrinterCode);

        return signingDevice;
    }
}
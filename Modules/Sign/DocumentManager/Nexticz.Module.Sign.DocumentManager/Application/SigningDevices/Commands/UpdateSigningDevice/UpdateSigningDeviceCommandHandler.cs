using ErrorOr;
using MediatR;
using Microsoft.Extensions.Logging;
using Nexticz.Lib.Shared.Errors;
using Nexticz.Module.Sign.DocumentManager.Application.Interfaces;
using Nexticz.Module.Sign.DocumentManager.Application.SigningDevices.Queries.GetSigningDeviceByCode;
using Nexticz.Module.Sign.DocumentManager.Domain.SigningDeviceAggregate;
using Nexticz.Module.Sign.DocumentManager.Domain.SigningDeviceAggregate.Events;

namespace Nexticz.Module.Sign.DocumentManager.Application.SigningDevices.Commands.UpdateSigningDevice;

internal class UpdateSigningDeviceCommandHandler(
    ILogger<UpdateSigningDeviceCommandHandler> logger,
    IDocumentManagerUnitOfWork unitOfWork,
    ISender sender) : IRequestHandler<UpdateSigningDeviceCommand, ErrorOr<Success>>
{
    public async Task<ErrorOr<Success>> Handle(UpdateSigningDeviceCommand request, CancellationToken cancellationToken)
    {
        var signingDevice = await sender.Send(new GetSigningDeviceByCodeQuery(request.Code), cancellationToken);

        if (!signingDevice.HasValue())
        {
            logger.LogWarning("SIGN - DocumentManager - object {ObjectName} with code: {SigningDeviceCode} does not exists. We cannot update it.", 
                nameof(SigningDevice), signingDevice.Value.Code);
            return SigningDeviceErrors.ValidationSigningDeviceWithCodeDoesNotExist;       
        }

        var signingDeviceUpdateEvent = new SigningDeviceUpdatedEvent(
            signingDevice.Value.Id, signingDevice.Value.Code, request.Name, request.IsActive, request.PrinterCode);
        
        unitOfWork.AppendEvent(
            signingDevice.Value.Id, signingDeviceUpdateEvent);
            
        logger.LogInformation("SIGN - DocumentManager - object {ObjectName} with Id: {SigningDeviceId} updated, " +
                              "code: {SigningDeviceCode}, name: {SigningDeviceName}, is active: {SigningDeviceIsActive},  printer code: {SigningDevicePrinterCode}.", 
            nameof(SigningDevice), signingDevice.Value.Id, signingDevice.Value.Code, request.Name, request.IsActive, request.PrinterCode);

        return Result.Success;
    }
}
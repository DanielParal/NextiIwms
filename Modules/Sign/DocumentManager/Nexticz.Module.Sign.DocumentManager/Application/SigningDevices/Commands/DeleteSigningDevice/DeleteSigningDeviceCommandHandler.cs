using ErrorOr;
using MediatR;
using Microsoft.Extensions.Logging;
using Nexticz.Lib.Shared.Errors;
using Nexticz.Module.Sign.DocumentManager.Application.Interfaces;
using Nexticz.Module.Sign.DocumentManager.Application.SigningDevices.Queries.GetSigningDeviceByCode;
using Nexticz.Module.Sign.DocumentManager.Domain.SigningDeviceAggregate;
using Nexticz.Module.Sign.DocumentManager.Domain.SigningDeviceAggregate.Events;

namespace Nexticz.Module.Sign.DocumentManager.Application.SigningDevices.Commands.DeleteSigningDevice;

internal class DeleteSigningDeviceCommandHandler(
    ILogger<DeleteSigningDeviceCommandHandler> logger,
    IDocumentManagerUnitOfWork unitOfWork,
    ISender sender) : IRequestHandler<DeleteSigningDeviceCommand, ErrorOr<Success>>
{
    public async Task<ErrorOr<Success>> Handle(DeleteSigningDeviceCommand request, CancellationToken cancellationToken)
    {
        var signingDevice = await sender.Send(new GetSigningDeviceByCodeQuery(request.Code), cancellationToken);

        if (!signingDevice.HasValue())
        {
            logger.LogWarning("SIGN - DocumentManager - object {ObjectName} with code: {SigningDeviceCode} does not exists. We cannot delete it.", 
                nameof(SigningDevice), signingDevice.Value.Code);
            return SigningDeviceErrors.ValidationSigningDeviceWithCodeDoesNotExist;       
        }

        var signingDeviceDeletedEvent = new SigningDeviceDeletedEvent(
            signingDevice.Value.Id, signingDevice.Value.Code);
        
        unitOfWork.AppendEvent(
            signingDevice.Value.Id, signingDeviceDeletedEvent);
            
        logger.LogInformation("SIGN - DocumentManager - object {ObjectName} with Id: {SigningDeviceId} deleted, code: {SigningDeviceCode}.", 
            nameof(SigningDevice), signingDevice.Value.Id, signingDevice.Value.Code);

        return Result.Success;
    }
}
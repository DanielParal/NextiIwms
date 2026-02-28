using ErrorOr;
using MediatR;
using Microsoft.Extensions.Logging;
using Nexticz.Module.Sign.DocumentManager.Application.Interfaces;
using Nexticz.Module.Sign.DocumentManager.Domain.SigningDeviceAggregate.Events;

namespace Nexticz.Module.Sign.DocumentManager.Application.SigningDevices.Commands.SaveSigningResult;

internal class SaveSigningResultCommandHandler(
    ILogger<SaveSigningResultCommandHandler> logger,
    IDocumentManagerUnitOfWork unitOfWork) : IRequestHandler<SaveSigningResultCommand, ErrorOr<Success>>
{
    public async Task<ErrorOr<Success>> Handle(SaveSigningResultCommand request, CancellationToken cancellationToken)
    {
        var documentsSignedWithSigningDeviceEvent = new SigningResultSavedEvent(
            request.SigningDeviceId, 
            request.SigningDeviceCode, 
            request.SigningResult, 
            request.ExecutedAt,
            request.SentToDeviceByUserName,
            request.ExecutedByDriverName,
            request.SentLoadingDocuments);
        
        unitOfWork.AppendEvent(
            request.SigningDeviceId, documentsSignedWithSigningDeviceEvent);
            
        logger.LogInformation("SIGN - DocumentManager - signing result saved for signing device with Id: {SigningDeviceId}, code: {SigningDeviceCode}, executed at: {ExecutedAt}.", 
            request.SigningDeviceId, request.SigningDeviceCode, request.ExecutedAt);
        
        return Result.Success;
    }
}
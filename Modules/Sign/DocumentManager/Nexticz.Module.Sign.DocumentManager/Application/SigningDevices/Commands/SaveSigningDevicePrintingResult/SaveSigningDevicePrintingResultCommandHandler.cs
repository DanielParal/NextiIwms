using ErrorOr;
using MediatR;
using Microsoft.Extensions.Logging;
using Nexticz.Module.Sign.DocumentManager.Application.Interfaces;
using Nexticz.Module.Sign.DocumentManager.Domain.SigningDeviceAggregate;
using Nexticz.Module.Sign.DocumentManager.Domain.SigningDeviceAggregate.Events;

namespace Nexticz.Module.Sign.DocumentManager.Application.SigningDevices.Commands.SaveSigningDevicePrintingResult;

internal class SaveSigningDevicePrintingResultCommandHandler(
    ILogger<SaveSigningDevicePrintingResultCommandHandler> logger,
    IDocumentManagerUnitOfWork unitOfWork) : IRequestHandler<SaveSigningDevicePrintingResultCommand, ErrorOr<Success>>
{
    public async Task<ErrorOr<Success>> Handle(SaveSigningDevicePrintingResultCommand request, CancellationToken cancellationToken)
    {
        var printingResult = new PrintingResult(request.PrintingResult);
        var signingDevicePrintingResultSavedEvent = new SigningDevicePrintingResultSavedEvent(
            request.SigningDeviceId, 
            request.SigningDeviceCode, 
            printingResult, 
            request.PrintedAt,
            request.DocumentPrintJobs);
        
        unitOfWork.AppendEvent(
            request.SigningDeviceId, signingDevicePrintingResultSavedEvent);
            
        logger.LogInformation("SIGN - DocumentManager - printing result saved for signing device with Id: {SigningDeviceId}, code: {SigningDeviceCode}, printed at: {ExecutedAt}.", 
            request.SigningDeviceId, request.SigningDeviceCode, request.PrintedAt);
        
        return Result.Success;
    }
}
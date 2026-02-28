using ErrorOr;
using MediatR;
using Nexticz.Module.Sign.DocumentManager.Application.LoadingDocuments.Commands.RevertLoadingDocumentsFiles;
using Nexticz.Module.Sign.DocumentManager.Contracts.SigningDevices;
using Nexticz.Module.Sign.DocumentManager.Application.MasstransitPublishers.SignalRNotifications;
using Nexticz.Module.Sign.DocumentManager.Application.SigningDevices.Commands.ReturnLoadingDocumentFromSigningDevice;
using Nexticz.Module.Sign.DocumentManager.Application.SigningDevices.Commands.SendLoadingDocumentToSigningDevice;
using Nexticz.Module.Sign.DocumentManager.Application.SigningDevices.Models;

namespace Nexticz.Module.Sign.DocumentManager.Application.SigningDevices.Orchestrators;

internal class DeviceCommunicationOrchestrator(
    ISender sender,
    IDocumentMovementNotifier documentMovementNotifier) : IDeviceCommunicationOrchestrator
{
    public async Task<ErrorOr<Success>> SendDocumentsToDeviceAsync(
        SendDocumentJobContract[] sendDocumentJobContracts, 
        string signingDeviceCode,
        string? driverName, 
        string? licensePlate,
        CancellationToken cancellationToken)
    {
        var upperSigningDeviceCode = signingDeviceCode.ToUpperInvariant();
        
        var result = 
            await sender.Send(new SendLoadingDocumentToSigningDeviceCommand(
                LoadingDocumentToSendFactory.Create(sendDocumentJobContracts), upperSigningDeviceCode, driverName, licensePlate), cancellationToken);
        
        if (result.IsError)
            return result;
        
        await documentMovementNotifier.NotifyDocumentsSentAsync(upperSigningDeviceCode, cancellationToken);
        return Result.Success;
    }

    public async Task<ErrorOr<Success>> ReturnDocumentsFromDeviceAsync(string signingDeviceCode, CancellationToken cancellationToken)
    {
        var upperSigningDeviceCode = signingDeviceCode.ToUpperInvariant();
        
        var result = await sender.Send(new ReturnLoadingDocumentFromSigningDeviceCommand(upperSigningDeviceCode), cancellationToken);
        
        if (result.IsError)
            return result;
        
        await documentMovementNotifier.NotifyDocumentsReturnedAsync(upperSigningDeviceCode, cancellationToken);
        return Result.Success;
    }
}
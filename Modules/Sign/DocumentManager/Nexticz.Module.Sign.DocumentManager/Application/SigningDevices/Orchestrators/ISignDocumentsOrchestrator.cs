using ErrorOr;
using Nexticz.Lib.Shared.FileHandling.Models;
using Nexticz.Module.Sign.DocumentManager.Application.SigningDevices.Models;

namespace Nexticz.Module.Sign.DocumentManager.Application.SigningDevices.Orchestrators;

internal interface ISignDocumentsOrchestrator
{
    Task<ErrorOr<Success>> SignAsync(
        string signingDeviceCode, 
        string currentUserName,
        string driverName, 
        string licensePlate, 
        FileResult signatureFile, 
        CancellationToken cancellationToken);
}
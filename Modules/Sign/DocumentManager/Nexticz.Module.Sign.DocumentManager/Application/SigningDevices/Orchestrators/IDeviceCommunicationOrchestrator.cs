using ErrorOr;
using Nexticz.Module.Sign.DocumentManager.Contracts.SigningDevices;

namespace Nexticz.Module.Sign.DocumentManager.Application.SigningDevices.Orchestrators;

internal interface IDeviceCommunicationOrchestrator
{
    Task<ErrorOr<Success>> SendDocumentsToDeviceAsync(SendDocumentJobContract[] sendDocumentJobContracts, string signingDeviceCode, string? driverName, string? licensePlate, CancellationToken cancellationToken);
    Task<ErrorOr<Success>> ReturnDocumentsFromDeviceAsync(string signingDeviceCode, CancellationToken cancellationToken);
}
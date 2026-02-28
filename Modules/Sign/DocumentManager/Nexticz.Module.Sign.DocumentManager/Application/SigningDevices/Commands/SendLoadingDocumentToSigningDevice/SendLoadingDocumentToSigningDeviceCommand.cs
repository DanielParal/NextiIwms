using ErrorOr;
using Nexticz.Module.Sign.DocumentManager.Application.SigningDevices.Models;

namespace Nexticz.Module.Sign.DocumentManager.Application.SigningDevices.Commands.SendLoadingDocumentToSigningDevice;

internal record SendLoadingDocumentToSigningDeviceCommand(
    LoadingDocumentToSend[] LoadingDocumentsToSend, string SigningDeviceCode, 
    string? DriverName, string? LicensePlate) : IDocumentManagerCommand<ErrorOr<Success>>;
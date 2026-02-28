using ErrorOr;

namespace Nexticz.Module.Sign.DocumentManager.Application.SigningDevices.Commands.ReturnLoadingDocumentFromSigningDevice;

internal record ReturnLoadingDocumentFromSigningDeviceCommand(string SigningDeviceCode) : IDocumentManagerCommand<ErrorOr<Success>>;
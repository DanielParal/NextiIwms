using ErrorOr;

namespace Nexticz.Module.Sign.DocumentManager.Application.SigningDevices.Commands.DeleteSigningDevice;

internal record DeleteSigningDeviceCommand(string Code) : IDocumentManagerCommand<ErrorOr<Success>>;
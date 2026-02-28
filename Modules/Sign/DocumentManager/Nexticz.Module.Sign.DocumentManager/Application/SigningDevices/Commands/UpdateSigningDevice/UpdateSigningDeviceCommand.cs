using ErrorOr;

namespace Nexticz.Module.Sign.DocumentManager.Application.SigningDevices.Commands.UpdateSigningDevice;

internal record UpdateSigningDeviceCommand(string Code, string Name, bool IsActive, string PrinterCode) : IDocumentManagerCommand<ErrorOr<Success>>;
using ErrorOr;

namespace Nexticz.Module.Sign.Settings.Application.SigningDevices.Commands.UpdateSigningDevice;

internal record UpdateSigningDeviceCommand(
    string Code, string Name, bool IsActive, string LocationCode, string PrinterCode) : ISettingsCommand<ErrorOr<Updated>>;
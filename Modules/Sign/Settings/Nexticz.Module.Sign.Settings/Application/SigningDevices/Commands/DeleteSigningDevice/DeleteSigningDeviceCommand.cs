using ErrorOr;

namespace Nexticz.Module.Sign.Settings.Application.SigningDevices.Commands.DeleteSigningDevice;

internal record DeleteSigningDeviceCommand(string Code) : ISettingsCommand<ErrorOr<Deleted>>;
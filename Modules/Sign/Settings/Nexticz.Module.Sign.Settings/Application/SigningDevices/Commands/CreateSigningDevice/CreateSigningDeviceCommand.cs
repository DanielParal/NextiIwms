using ErrorOr;
using Nexticz.Module.Sign.Settings.Domain.SigningDeviceAggregate;

namespace Nexticz.Module.Sign.Settings.Application.SigningDevices.Commands.CreateSigningDevice;

internal record CreateSigningDeviceCommand(
    string Code, string Name, bool IsActive, string LocationCode, string PrinterCode) : ISettingsCommand<ErrorOr<SigningDevice>>;
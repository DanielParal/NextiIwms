using Nexticz.Module.Sign.Settings.Contracts.SigningDevices;
using Nexticz.Module.Sign.Settings.Domain.SigningDeviceAggregate;

namespace Nexticz.Module.Sign.Settings.Presentation.SigningDevices;

internal static class SigningDeviceResponseFactory
{
    public static SigningDeviceResponse Create(SigningDevice signingDevice)
    {
        return new SigningDeviceResponse(
            signingDevice.Id, signingDevice.Code, signingDevice.Name, signingDevice.IsActive, 
            signingDevice.LocationCode,signingDevice.PrinterCode);
    }
}
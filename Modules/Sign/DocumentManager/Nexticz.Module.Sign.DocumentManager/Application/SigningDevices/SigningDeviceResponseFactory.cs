using Nexticz.Module.Sign.Settings.Contracts.SigningDevices;
using SigningDeviceResponse = Nexticz.Module.Sign.DocumentManager.Contracts.SigningDevices.SigningDeviceResponse;

namespace Nexticz.Module.Sign.DocumentManager.Application.SigningDevices;

internal static class SigningDeviceResponseFactory
{
    public static SigningDeviceResponse Create(SigningDeviceContract signingDeviceContract)
    {
        return new SigningDeviceResponse(
            signingDeviceContract.Id,
            signingDeviceContract.Code,
            signingDeviceContract.Name,
            false);
    }
}
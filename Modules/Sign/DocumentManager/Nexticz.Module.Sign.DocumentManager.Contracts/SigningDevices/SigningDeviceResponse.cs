using System.ComponentModel.DataAnnotations;

namespace Nexticz.Module.Sign.DocumentManager.Contracts.SigningDevices;

public record SigningDeviceResponse(
    [property: Required] Guid Id, 
    [property: Required] string Code, 
    [property: Required] string Name, 
    [property: Required] bool IsBusy);
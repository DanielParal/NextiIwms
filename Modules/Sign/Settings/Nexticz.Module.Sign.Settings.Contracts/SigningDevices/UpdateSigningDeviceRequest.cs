using System.ComponentModel.DataAnnotations;

namespace Nexticz.Module.Sign.Settings.Contracts.SigningDevices;

public record UpdateSigningDeviceRequest(
    [property: Required] string Name, 
    [property: Required] bool IsActive,
    [property: Required] string LocationCode,
    [property: Required] string PrinterCode);
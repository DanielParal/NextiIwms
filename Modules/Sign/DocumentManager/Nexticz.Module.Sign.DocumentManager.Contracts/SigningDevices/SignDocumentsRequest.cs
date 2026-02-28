using System.ComponentModel.DataAnnotations;

namespace Nexticz.Module.Sign.DocumentManager.Contracts.SigningDevices;

public record SignDocumentsRequest(
    [property: Required] string DriverName, 
    [property: Required] string LicensePlate);
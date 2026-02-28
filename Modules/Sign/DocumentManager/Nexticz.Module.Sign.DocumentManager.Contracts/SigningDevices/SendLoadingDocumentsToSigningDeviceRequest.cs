using System.ComponentModel.DataAnnotations;

namespace Nexticz.Module.Sign.DocumentManager.Contracts.SigningDevices;

public record SendLoadingDocumentsToSigningDeviceRequest(
    string? DriverName,
    string? LicensePlate,
    [property: Required] SendDocumentJobContract[] SendDocumentJobs);
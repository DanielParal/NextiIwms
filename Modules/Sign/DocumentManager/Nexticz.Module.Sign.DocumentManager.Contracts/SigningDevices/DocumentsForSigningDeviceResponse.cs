using System.ComponentModel.DataAnnotations;

namespace Nexticz.Module.Sign.DocumentManager.Contracts.SigningDevices;

public record DocumentsForSigningDeviceResponse(
    string? LicensePlate,
    string? DriverName,
    decimal? TotalWeight,
    int? TotalAdrPoints,
    [property: Required] LoadingDocumentToSignContract[] LoadingDocumentsToSign);
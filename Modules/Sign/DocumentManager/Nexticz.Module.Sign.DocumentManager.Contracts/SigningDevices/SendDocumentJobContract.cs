using System.ComponentModel.DataAnnotations;

namespace Nexticz.Module.Sign.DocumentManager.Contracts.SigningDevices;

public record SendDocumentJobContract(
    [property: Required] string LoadingDocumentCode,
    string? DeliveryDocumentCode);
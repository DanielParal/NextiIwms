using System.ComponentModel.DataAnnotations;

namespace Nexticz.Module.Sign.DocumentManager.Contracts.Emails;

public record SendEmailJobContract(
    [property: Required] string LoadingDocumentCode,
    string? DeliveryDocumentCode);
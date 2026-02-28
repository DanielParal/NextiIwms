using System.ComponentModel.DataAnnotations;

namespace Nexticz.Module.Sign.DocumentManager.Contracts.SigningDevices;

public record DeliveryDocumentToSignContract(
    [property: Required] string Code,
    [property: Required] string PartnersOrderNumber);
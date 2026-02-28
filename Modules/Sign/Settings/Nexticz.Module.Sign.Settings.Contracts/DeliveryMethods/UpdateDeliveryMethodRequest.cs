using System.ComponentModel.DataAnnotations;

namespace Nexticz.Module.Sign.Settings.Contracts.DeliveryMethods;

public record UpdateDeliveryMethodRequest(
    [property: Required] string Name,
    [property: Required] int LoadingDocumentPrintCopiesCount,
    [property: Required] int DeliveryDocumentPrintCopiesCount);
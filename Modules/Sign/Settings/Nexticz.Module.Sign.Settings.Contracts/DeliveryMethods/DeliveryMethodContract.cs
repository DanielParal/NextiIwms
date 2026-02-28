namespace Nexticz.Module.Sign.Settings.Contracts.DeliveryMethods;

public record DeliveryMethodContract(
    string Code, 
    string Name,
    int LoadingDocumentPrintCopiesCount,
    int DeliveryDocumentPrintCopiesCount);
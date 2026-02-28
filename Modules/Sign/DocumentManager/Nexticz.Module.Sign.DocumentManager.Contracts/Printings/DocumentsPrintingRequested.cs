namespace Nexticz.Module.Sign.DocumentManager.Contracts.Printings;

public record DocumentsPrintingRequested(Guid SigningDeviceId, string SigningDeviceCode, DateTimeOffset PrintDeadline, string PrinterCode, DocumentPrintJobContract[] DocumentPrintJobs);

public record DocumentPrintJobContract(string LoadingDocumentCode, string? DeliveryDocumentCode, int CopiesCount);
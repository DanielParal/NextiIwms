using Nexticz.Module.Sign.DocumentManager.Domain.PrinterAggregate;

namespace Nexticz.Module.Sign.DocumentManager.Domain.SigningDeviceAggregate.Events;

public record SigningDevicePrintingResultSavedEvent(
    Guid Id,
    string Code,
    PrintingResult PrintingResult,
    DateTimeOffset PrintedAt,
    DocumentPrintJob[] DocumentPrintJobs);
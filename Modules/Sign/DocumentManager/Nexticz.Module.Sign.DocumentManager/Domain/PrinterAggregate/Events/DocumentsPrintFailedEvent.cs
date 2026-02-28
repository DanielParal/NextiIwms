using Nexticz.Lib.Shared.DataAccess.Marten.Configuration;

namespace Nexticz.Module.Sign.DocumentManager.Domain.PrinterAggregate.Events;

public record DocumentsPrintFailedEvent(
    Guid PrinterId, string PrinterCode, string PrinterIp, DocumentPrintJob[] PrintedDocuments, string ErrorCode, string ErrorMessage) : IMartenEvent;
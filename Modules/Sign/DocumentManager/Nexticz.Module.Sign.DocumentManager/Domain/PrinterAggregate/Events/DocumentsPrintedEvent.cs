using Nexticz.Lib.Shared.DataAccess.Marten.Configuration;

namespace Nexticz.Module.Sign.DocumentManager.Domain.PrinterAggregate.Events;

public record DocumentsPrintedEvent(
    Guid PrinterId, string PrinterCode, string PrinterIp, DocumentPrintJob[] PrintedDocuments, TimeSpan? PrintTimeDuration) : IMartenEvent;
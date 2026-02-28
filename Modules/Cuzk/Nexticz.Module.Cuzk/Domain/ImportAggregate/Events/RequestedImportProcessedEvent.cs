using Nexticz.Lib.Shared.DataAccess.Marten.Configuration;

namespace Nexticz.Module.Cuzk.Domain.ImportAggregate.Events;

public record RequestedImportProcessedEvent(
    Guid Id, ImportStatus Status, ImportError[] Errors, 
    DateTimeOffset DateImported, string[] ImportedCodes) : IMartenEvent;
using Nexticz.Lib.Shared.DataAccess.Marten.Configuration;

namespace Nexticz.Module.Cuzk.Domain.ImportAggregate.Events;

public record ImportRequestedEvent(
    Guid Id, string UserName, ImportType Type, string FileName, string? CsvDelimiter, DateTimeOffset DateRequested) : IMartenEvent;
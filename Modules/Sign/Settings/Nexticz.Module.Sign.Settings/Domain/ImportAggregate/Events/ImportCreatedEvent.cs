using Nexticz.Lib.Shared.DataAccess.Marten.Configuration;

namespace Nexticz.Module.Sign.Settings.Domain.ImportAggregate.Events;

public record ImportCreatedEvent(
    Guid Id, string UserName, ImportType Type, ImportStatus Status, ImportError[] Errors, 
    DateTimeOffset DateCreated, string[] ImportedCodes) : IMartenEvent;
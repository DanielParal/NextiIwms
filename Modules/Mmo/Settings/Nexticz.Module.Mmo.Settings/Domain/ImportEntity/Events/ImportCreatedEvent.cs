using Nexticz.Lib.Shared.DataAccess.Marten.Configuration;

namespace Nexticz.Module.Mmo.Settings.Domain.ImportEntity.Events;

public record ImportCreatedEvent(Guid Id, string UserName, ImportType Type, ImportStatus Status, ImportError[] Errors, DateTimeOffset DateCreated, string[] ImportedCodes) : IMartenEvent;
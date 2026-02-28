using Nexticz.Lib.Shared.DataAccess.Marten.Configuration;

namespace Nexticz.Module.Mmo.Settings.Domain.ExportEntity.Events;

public record ExportCreatedEvent(Guid Id, string UserName, ExportType Type, DateTimeOffset DateCreated) : IMartenEvent;
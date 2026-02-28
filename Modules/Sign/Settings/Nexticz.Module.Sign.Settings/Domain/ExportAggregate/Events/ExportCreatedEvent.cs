using Nexticz.Lib.Shared.DataAccess.Marten.Configuration;

namespace Nexticz.Module.Sign.Settings.Domain.ExportAggregate.Events;

public record ExportCreatedEvent(Guid Id, string UserName, ExportType Type, DateTimeOffset DateCreated) : IMartenEvent;
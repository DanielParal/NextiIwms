using Nexticz.Lib.Shared.DataAccess.Marten.Configuration;

namespace Nexticz.Module.Mmo.Settings.Domain.WorkerEntity.Events;

public record WorkerCreatedEvent(Guid Id, string Name, int Pin, bool IsActive) : IMartenEvent;
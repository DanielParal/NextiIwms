using Nexticz.Lib.Shared.DataAccess.Marten.Configuration;

namespace Nexticz.Module.Mmo.Settings.Domain.WorkerEntity.Events;

public record WorkerDeletedEvent(Guid Id) : IMartenEvent;
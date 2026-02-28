using Nexticz.Lib.Shared.DataAccess.Marten.Configuration;

namespace Nexticz.Module.Mmo.Settings.Domain.InactivityTypeEntity.Events;

public record InactivityTypeDeletedEvent(Guid Id) : IMartenEvent;
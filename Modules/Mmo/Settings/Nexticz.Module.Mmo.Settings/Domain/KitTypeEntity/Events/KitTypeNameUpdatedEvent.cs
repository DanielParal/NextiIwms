using Nexticz.Lib.Shared.DataAccess.Marten.Configuration;

namespace Nexticz.Module.Mmo.Settings.Domain.KitTypeEntity.Events;

public record KitTypeNameUpdatedEvent(Guid Id, string Name) : IMartenEvent;
using Nexticz.Lib.Shared.DataAccess.Marten.Configuration;

namespace Nexticz.Module.Mmo.Settings.Domain.KitSapDefinitionEntity.Events;

public record KitSapDefinitionNameUpdatedEvent(Guid Id, string Name) : IMartenEvent;
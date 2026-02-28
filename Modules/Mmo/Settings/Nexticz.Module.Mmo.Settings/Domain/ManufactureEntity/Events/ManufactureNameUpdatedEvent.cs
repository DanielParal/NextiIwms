using Nexticz.Lib.Shared.DataAccess.Marten.Configuration;

namespace Nexticz.Module.Mmo.Settings.Domain.ManufactureEntity.Events;

public record ManufactureNameUpdatedEvent(Guid Id, string Name) : IMartenEvent;
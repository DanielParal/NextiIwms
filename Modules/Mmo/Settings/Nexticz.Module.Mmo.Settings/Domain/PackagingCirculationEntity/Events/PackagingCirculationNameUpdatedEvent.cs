using Nexticz.Lib.Shared.DataAccess.Marten.Configuration;

namespace Nexticz.Module.Mmo.Settings.Domain.PackagingCirculationEntity.Events;

public record PackagingCirculationNameUpdatedEvent(Guid Id, string Name) : IMartenEvent;
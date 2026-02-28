using Nexticz.Lib.Shared.DataAccess.Marten.Configuration;

namespace Nexticz.Module.Mmo.Settings.Domain.PackagingTypeEntity.Events;

public record PackagingTypeNameUpdatedEvent(Guid Id, string Name) : IMartenEvent;
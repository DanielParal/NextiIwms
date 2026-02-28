using Nexticz.Lib.Shared.DataAccess.Marten.Configuration;

namespace Nexticz.Module.Mmo.Settings.Domain.SpecialInformationEntity.Events;

public record SpecialInformationCreatedEvent(Guid Id, string Title, string Description) : IMartenEvent;
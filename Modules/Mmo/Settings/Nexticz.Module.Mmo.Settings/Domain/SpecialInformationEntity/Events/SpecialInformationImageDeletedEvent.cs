using Nexticz.Lib.Shared.DataAccess.Marten.Configuration;

namespace Nexticz.Module.Mmo.Settings.Domain.SpecialInformationEntity.Events;

public record SpecialInformationImageDeletedEvent(Guid Id) : IMartenEvent;
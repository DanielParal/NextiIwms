using Nexticz.Lib.Shared.DataAccess.Marten.Configuration;

namespace Nexticz.Module.Mmo.Settings.Domain.SpecialInformationEntity.Events;

public record SpecialInformationImageUploadedEvent(Guid Id) : IMartenEvent;
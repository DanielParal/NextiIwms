using Nexticz.Lib.Shared.DataAccess.Marten.Configuration;

namespace Nexticz.Module.Mmo.Settings.Domain.DepositorEntity.Events;

public record DepositorUpdatedEvent(Guid Id, string Name, string? BarcodeTemplate) : IMartenEvent;
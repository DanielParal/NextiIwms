using Nexticz.Lib.Shared.DataAccess.Marten.Configuration;
using Nexticz.Module.Mmo.SharedKernel;
using Nexticz.Module.Mmo.SharedKernel.DomainCore;

namespace Nexticz.Module.Mmo.Settings.Domain.DepositorEntity.Events;

public class DepositorCreatedEvent(Guid id, string code, string name, string? barcodeTemplate)
    : EventWithCode(code), IMartenEvent
{
    public Guid Id { get; } = id;
    public string Name { get; } = name;
    public string? BarcodeTemplate { get; } = barcodeTemplate;
}

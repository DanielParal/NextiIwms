using Nexticz.Lib.Shared.DataAccess.Marten.Configuration;
using Nexticz.Module.Mmo.SharedKernel;
using Nexticz.Module.Mmo.SharedKernel.DomainCore;

namespace Nexticz.Module.Mmo.Settings.Domain.DepositorEntity.Events;

public class DepositorDeletedEvent(Guid id, string code) : EventWithCode(code), IMartenEvent
{
    public Guid Id { get; } = id;
}
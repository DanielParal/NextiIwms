using Nexticz.Lib.Shared.DataAccess.Marten.Configuration;
using Nexticz.Module.Sign.SharedKernel;
using Nexticz.Module.Sign.SharedKernel.DomainCore;

namespace Nexticz.Module.Sign.Settings.Domain.DepositorGroupAggregate.Events;

public class DepositorGroupUpdatedEvent(Guid id, string code, string name) : EventWithCode(code), IMartenEvent
{
    public Guid Id { get; } = id;
    public string Name { get; } = name;
}
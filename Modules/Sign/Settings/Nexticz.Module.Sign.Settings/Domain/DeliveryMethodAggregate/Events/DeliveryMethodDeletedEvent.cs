using Nexticz.Lib.Shared.DataAccess.Marten.Configuration;
using Nexticz.Module.Sign.SharedKernel;
using Nexticz.Module.Sign.SharedKernel.DomainCore;

namespace Nexticz.Module.Sign.Settings.Domain.DeliveryMethodAggregate.Events;

public class DeliveryMethodDeletedEvent(Guid id, string code) : EventWithCode(code), IMartenEvent
{
    public Guid Id { get; } = id;
}
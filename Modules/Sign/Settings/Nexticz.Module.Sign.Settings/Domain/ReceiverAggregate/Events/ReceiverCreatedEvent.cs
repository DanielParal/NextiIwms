using Nexticz.Lib.Shared.DataAccess.Marten.Configuration;
using Nexticz.Module.Sign.SharedKernel.DomainCore;

namespace Nexticz.Module.Sign.Settings.Domain.ReceiverAggregate.Events;

public class ReceiverCreatedEvent(Guid id, string code, string partnerCode, string name)
    : EventWithCode(code), IMartenEvent
{
    public Guid Id { get; } = id;
    public string PartnerCode { get; } = partnerCode;
    public string Name { get; } = name;
}
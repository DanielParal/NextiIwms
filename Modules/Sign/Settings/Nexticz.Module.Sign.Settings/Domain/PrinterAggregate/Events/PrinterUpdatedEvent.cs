using Nexticz.Lib.Shared.DataAccess.Marten.Configuration;
using Nexticz.Module.Sign.SharedKernel.DomainCore;

namespace Nexticz.Module.Sign.Settings.Domain.PrinterAggregate.Events;

public class PrinterUpdatedEvent(Guid id, string code, string name, string ip) : EventWithCode(code), IMartenEvent
{
    public Guid Id { get; } = id;
    public string Name { get; } = name;
    public string Ip { get; } = ip;
}
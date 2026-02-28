using Nexticz.Module.Sign.Settings.Domain.PrinterAggregate.Events;
using Nexticz.Module.Sign.SharedKernel.DomainCore;

namespace Nexticz.Module.Sign.Settings.Domain.PrinterAggregate;

public class Printer : AggregateRoot
{
    public string Code { get; private set; }
    public string Name { get; private set; }
    public string Ip { get; private set; }
    
    // We need private constructor due to Marten deserialization
    private Printer() {}
    
    public Printer(
        string code, 
        string name, 
        string ip,
        Guid? id = null) : base(id ?? Guid.NewGuid())
    {
        Code = code.ToUpperInvariant();
        Name = name;
        Ip = ip;
    }

    public void Apply(PrinterCreatedEvent @event)
    {
        Code = @event.Code;
        Name = @event.Name;
        Ip = @event.Ip;
    }
    
    public void Apply(PrinterUpdatedEvent @event)
    {
        Name = @event.Name;
        Ip = @event.Ip;
    }
}
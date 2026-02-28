using Nexticz.Module.Sign.DocumentManager.Domain.PrinterAggregate.Events;
using Nexticz.Module.Sign.SharedKernel.DomainCore;

namespace Nexticz.Module.Sign.DocumentManager.Domain.PrinterAggregate;

public class Printer : AggregateRoot
{
    public string Code { get; private set; }
    public long PrintedCopies { get; private set; }
    public int PrintFailures { get; private set; }
    
    // We need private constructor due to Marten deserialization
    private Printer() {}

    public Printer(string code,       
        Guid? id = null) : base(Guid.NewGuid())
    {
        Code = code.ToUpperInvariant();
    }

    public void Apply(PrinterCreatedEvent @event)
    {
        Id = @event.Id;
        Code = @event.Code;
        PrintedCopies = 0;
        PrintFailures = 0;
    }
    
    public void Apply(DocumentsPrintedEvent @event)
    {
        PrintedCopies += @event.PrintedDocuments.Sum(x => x.CopiesCount);
    }
    
    public void Apply(DocumentsPrintFailedEvent @event)
    {
        PrintFailures++;
    }
}
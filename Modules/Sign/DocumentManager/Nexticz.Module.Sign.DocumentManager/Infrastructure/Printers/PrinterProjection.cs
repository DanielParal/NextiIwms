using JasperFx.Events;
using Marten.Events.Aggregation;
using Nexticz.Module.Sign.DocumentManager.Domain.PrinterAggregate;
using Nexticz.Module.Sign.DocumentManager.Domain.PrinterAggregate.Events;

namespace Nexticz.Module.Sign.DocumentManager.Infrastructure.Printers;

public class PrinterProjection : SingleStreamProjection<Printer, Guid>
{
    public void Apply(IEvent<PrinterCreatedEvent> @event, Printer printer)
    {
        printer.Apply(@event.Data);
    }
    
    public void Apply(IEvent<DocumentsPrintedEvent> @event, Printer printer)
    {
        printer.Apply(@event.Data);
    }
    
    public void Apply(IEvent<DocumentsPrintFailedEvent> @event, Printer printer)
    {
        printer.Apply(@event.Data);
    }
}
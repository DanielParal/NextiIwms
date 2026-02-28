using JasperFx.Events;
using Marten.Events.Aggregation;
using Nexticz.Module.Sign.Settings.Domain.PrinterAggregate;
using Nexticz.Module.Sign.Settings.Domain.PrinterAggregate.Events;

namespace Nexticz.Module.Sign.Settings.Infrastructure.Printers;

public class PrinterProjection : SingleStreamProjection<Printer, Guid>
{
    public PrinterProjection()
    {
        DeleteEvent<PrinterDeletedEvent>();
    }
    
    public void Apply(IEvent<PrinterCreatedEvent> @event, Printer printer)
    {
        printer.Apply(@event.Data);
    }
    
    public void Apply(IEvent<PrinterUpdatedEvent> @event, Printer printer)
    {
        printer.Apply(@event.Data);
    }
}
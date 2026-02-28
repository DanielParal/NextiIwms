using JasperFx.Events;
using Marten.Events.Aggregation;
using Nexticz.Module.Cuzk.Domain.ImportAggregate;
using Nexticz.Module.Cuzk.Domain.ImportAggregate.Events;

namespace Nexticz.Module.Cuzk.Infrastructure.Imports;

public class ImportProjection : SingleStreamProjection<Import, Guid>
{
    public void Apply(IEvent<ImportRequestedEvent> @event, Import import)
    {
        import.Apply(@event.Data);
    }
    
    public void Apply(IEvent<RequestedImportProcessedEvent> @event, Import import)
    {
        import.Apply(@event.Data);
    }
}
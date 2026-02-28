using JasperFx.Events;
using Marten.Events.Aggregation;
using Nexticz.Module.Sign.Settings.Domain.ExportAggregate;
using Nexticz.Module.Sign.Settings.Domain.ExportAggregate.Events;

namespace Nexticz.Module.Sign.Settings.Infrastructure.Exports;

public class ExportProjection : SingleStreamProjection<Export, Guid>
{
    public void Apply(IEvent<ExportCreatedEvent> @event, Export export)
    {
        export.Apply(@event.Data);
    }
}
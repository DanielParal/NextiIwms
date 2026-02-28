using JasperFx.Events;
using Marten.Events.Aggregation;
using Nexticz.Module.Mmo.Settings.Domain.ExportEntity;
using Nexticz.Module.Mmo.Settings.Domain.ExportEntity.Events;

namespace Nexticz.Module.Mmo.Settings.Infrastructure.Exports;

public class ExportProjection : SingleStreamProjection<Export, Guid>
{
    public void Apply(IEvent<ExportCreatedEvent> @event, Export export)
    {
        export.Apply(@event.Data);
    }
}
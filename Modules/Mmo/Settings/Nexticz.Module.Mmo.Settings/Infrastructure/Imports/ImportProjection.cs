using JasperFx.Events;
using Marten.Events.Aggregation;
using Nexticz.Module.Mmo.Settings.Domain.ImportEntity;
using Nexticz.Module.Mmo.Settings.Domain.ImportEntity.Events;

namespace Nexticz.Module.Mmo.Settings.Infrastructure.Imports;

public class ImportProjection : SingleStreamProjection<Import, Guid>
{
    public void Apply(IEvent<ImportCreatedEvent> @event, Import import)
    {
        import.Apply(@event.Data);
    }
}
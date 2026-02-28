using JasperFx.Events;
using Marten.Events.Aggregation;
using Nexticz.Module.Sign.Settings.Domain.ImportAggregate;
using Nexticz.Module.Sign.Settings.Domain.ImportAggregate.Events;

namespace Nexticz.Module.Sign.Settings.Infrastructure.Imports;

public class ImportProjection : SingleStreamProjection<Import, Guid>
{
    public void Apply(IEvent<ImportCreatedEvent> @event, Import import)
    {
        import.Apply(@event.Data);
    }
}
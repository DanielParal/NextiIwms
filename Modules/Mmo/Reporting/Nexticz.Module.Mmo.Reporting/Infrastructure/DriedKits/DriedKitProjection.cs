using JasperFx.Events;
using Marten.Events.Aggregation;
using Nexticz.Module.Mmo.Reporting.Domain.DriedKitAggregate;
using Nexticz.Module.Mmo.Reporting.Domain.DriedKitAggregate.Events;

namespace Nexticz.Module.Mmo.Reporting.Infrastructure.DriedKits;

public class DriedKitProjection : SingleStreamProjection<DriedKit, Guid>
{
    public void Apply(IEvent<DriedKitCreatedEvent> @event, DriedKit driedKit)
    {
        driedKit.Apply(@event.Data);
    }
}
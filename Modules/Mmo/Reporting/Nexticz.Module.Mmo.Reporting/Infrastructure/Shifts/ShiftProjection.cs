using JasperFx.Events;
using Marten.Events;
using Marten.Events.Aggregation;
using Nexticz.Module.Mmo.Reporting.Domain.ShiftAggregate;
using Nexticz.Module.Mmo.Reporting.Domain.ShiftAggregate.Events;

namespace Nexticz.Module.Mmo.Reporting.Infrastructure.Shifts;

public class ShiftProjection : SingleStreamProjection<Shift, Guid>
{
    public void Apply(IEvent<ShiftCreatedEvent> @event, Shift shift)
    {
        shift.Apply(@event.Data);
    }
    
    public void Apply(IEvent<LastShiftMovedToNextToLastEvent> @event, Shift shift)
    {
        shift.Apply(@event.Data);
    }
    
    public void Apply(IEvent<NextToLastShiftMovedBackEvent> @event, Shift shift)
    {
        shift.Apply(@event.Data);
    }
    
    public void Apply(IEvent<ShiftApprovedEvent> @event, Shift shift)
    {
        shift.Apply(@event.Data);
    }
    
    public void Apply(IEvent<ShiftUnapprovedEvent> @event, Shift shift)
    {
        shift.Apply(@event.Data);
    }
}
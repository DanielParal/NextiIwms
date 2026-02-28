using JasperFx.Events;
using Marten.Events.Aggregation;
using Nexticz.Module.Sign.Settings.Domain.ConstantAggregate;
using Nexticz.Module.Sign.Settings.Domain.ConstantAggregate.Events;

namespace Nexticz.Module.Sign.Settings.Infrastructure.Constants;

public class ConstantProjection : SingleStreamProjection<Constant, Guid>
{
    public void Apply(IEvent<ConstantCreatedEvent> @event, Constant constant)
    {
        constant.Apply(@event.Data);
    }
    
    public void Apply(IEvent<ConstantUpdatedEvent> @event, Constant constant)
    {
        constant.Apply(@event.Data);
    }
    
    public Constant? Apply(IEvent<ConstantDeletedEvent> @event, Constant constant)
    {
        return null;
    }
}
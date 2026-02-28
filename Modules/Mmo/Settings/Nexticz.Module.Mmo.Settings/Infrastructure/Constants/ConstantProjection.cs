using JasperFx.Events;
using Marten.Events.Aggregation;
using Nexticz.Module.Mmo.Settings.Domain.ConstantEntity;
using Nexticz.Module.Mmo.Settings.Domain.ConstantEntity.Events;

namespace Nexticz.Module.Mmo.Settings.Infrastructure.Constants;

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
using JasperFx.Events;
using Marten.Events.Aggregation;
using Nexticz.Module.Sign.Settings.Domain.PartnerAggregate;
using Nexticz.Module.Sign.Settings.Domain.PartnerAggregate.Events;

namespace Nexticz.Module.Sign.Settings.Infrastructure.Partners;

public class PartnerProjection : SingleStreamProjection<Partner, Guid>
{
    public PartnerProjection()
    {
        DeleteEvent<PartnerDeletedEvent>();
    }
    
    public void Apply(IEvent<PartnerCreatedEvent> @event, Partner partner)
    {
        partner.Apply(@event.Data);
    }
    
    public void Apply(IEvent<PartnerUpdatedEvent> @event, Partner partner)
    {
        partner.Apply(@event.Data);
    }
}
using Marten.Events.Projections;
using Nexticz.Module.Mmo.Reporting.Domain.KitAggregate.Events;
using Nexticz.Module.Mmo.Reporting.Domain.Views;

namespace Nexticz.Module.Mmo.Reporting.Infrastructure.LastItemPerLineViews;

public class LastItemPerLineProjection : MultiStreamProjection<LastItemPerLineView, string>
{
    public LastItemPerLineProjection()
    {
        Identity<KitCreatedEvent>(x => x.LineCode);
        
        ProjectEvent<KitCreatedEvent>((lastItemPerLineView, currentEvent) =>
        {
            lastItemPerLineView.ItemId = currentEvent.Id;
            lastItemPerLineView.ShiftId = currentEvent.ShiftId;
            lastItemPerLineView.LastFinishedDate = currentEvent.WashingEnded;
            lastItemPerLineView.CreatedAt = currentEvent.CreatedAt;
        });
    }
}
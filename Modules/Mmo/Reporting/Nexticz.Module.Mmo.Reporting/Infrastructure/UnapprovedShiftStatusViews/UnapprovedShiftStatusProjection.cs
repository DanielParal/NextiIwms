using Marten.Events.Projections;
using Nexticz.Module.Mmo.Reporting.Domain.ShiftAggregate;
using Nexticz.Module.Mmo.Reporting.Domain.ShiftAggregate.Events;
using Nexticz.Module.Mmo.Reporting.Domain.Views;

namespace Nexticz.Module.Mmo.Reporting.Infrastructure.UnapprovedShiftStatusViews;

public class UnapprovedShiftStatusProjection : MultiStreamProjection<UnapprovedShiftStatusView, Guid>
{
    public UnapprovedShiftStatusProjection()
    {
        Identity<ShiftStatusChangedEvent>(x => GenerateProjectionId(x.ShiftId));    
        Identity<ShiftApprovedEvent>(x => GenerateProjectionId(x.Id));    
        Identity<ShiftCreatedEvent>(x => GenerateProjectionId(x.Id));
        
        DeleteEvent<ShiftApprovedEvent>();

        ProjectEvent<ShiftCreatedEvent>((view, currentEvent) =>
        {
            view.ShiftId = currentEvent.Id;
            view.Status = ShiftStatus.NotApprovedWithoutIssues;
        });
        
        ProjectEvent<ShiftStatusChangedEvent>((view, currentEvent) =>
        {
            view.ShiftId = currentEvent.ShiftId;
            view.Status = currentEvent.ToStatus;
        });
    }
    
    private static Guid GenerateProjectionId(Guid shiftId)
    {
        var newIdWithPrefix = $"ShiftStatus-{shiftId}";
        var bytes = System.Text.Encoding.UTF8.GetBytes(newIdWithPrefix);
        var hash = System.Security.Cryptography.MD5.HashData(bytes);
        return new Guid(hash);
    }
}
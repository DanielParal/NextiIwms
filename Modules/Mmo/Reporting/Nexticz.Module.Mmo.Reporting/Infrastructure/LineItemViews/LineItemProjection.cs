using Marten.Events.Projections;
using Nexticz.Module.Mmo.Reporting.Domain.InactivityAggregate.Events;
using Nexticz.Module.Mmo.Reporting.Domain.KitAggregate.Events;
using Nexticz.Module.Mmo.Reporting.Domain.Views;

namespace Nexticz.Module.Mmo.Reporting.Infrastructure.LineItemViews;

public class LineItemProjection : MultiStreamProjection<LineItemView, Guid>
{
    public LineItemProjection()
    {
        Identity<KitCreatedEvent>(x => x.Id);
        Identity<KitCommentChangedEvent>(x => x.Id);
        Identity<InactivityCreatedEvent>(x => x.Id);
        Identity<InactivityCommentAddedEvent>(x => x.Id);
        Identity<InactivityTypeChangedEvent>(x => x.Id);
        Identity<InactivityTimeIntervalChangedEvent>(x => x.Id);
        Identity<KitTimeIntervalChangedEvent>(x => x.Id);
        Identity<KitDeletedEvent>(x => x.Id);
        Identity<InactivityUnplannedEvent>(x => x.Id);
        
        DeleteEvent<KitDeletedEvent>();
        
        ProjectEvent<KitCreatedEvent>((lineItemView, currentEvent) => 
        { 
            lineItemView.ShiftId = currentEvent.ShiftId;
            lineItemView.WashingMachineCode = currentEvent.WashingMachineCode;
            lineItemView.LineCode = currentEvent.LineCode;
            lineItemView.StartDate = currentEvent.WashingStarted;
            lineItemView.EndDate = currentEvent.WashingEnded;
            lineItemView.BatchId = currentEvent.BatchId;
            lineItemView.SisterKitId = currentEvent.SisterBatchId;
            lineItemView.KitId = currentEvent.KitId;
            lineItemView.SisterKitId = currentEvent.SisterKitId;
            lineItemView.KitCode = currentEvent.KitCode;
            lineItemView.PackagingCode = currentEvent.PackagingCode;
            lineItemView.OptimalPackagingSpeedOnWashingMachine = currentEvent.OptimalPackagingSpeedOnWashingMachine;
            lineItemView.OptimalPackagingSpeedOnWashingMachineLevel = currentEvent.OptimalPackagingSpeedOnWashingMachineLevel;
            lineItemView.KitNumber = currentEvent.KitNumber;
            lineItemView.KitOrderId = currentEvent.KitOrderId;
            lineItemView.TotalPlannedKitsCountInBatch = currentEvent.TotalPlannedKitsCountInBatch;
            lineItemView.WashingMachineSpeed = currentEvent.WashingMachineSpeed;
            lineItemView.WashingMachineSpeedLevel = currentEvent.WashingMachineSpeedLevel;
            lineItemView.KitEfficiency = currentEvent.Efficiency;
            lineItemView.RealTimeKitDuration = currentEvent.RealTimeKitDuration;
            lineItemView.OptimalKitDuration = currentEvent.OptimalKitDuration;
            lineItemView.Type = LineItemType.Kit;
            lineItemView.IsPlanned = false;
            lineItemView.AffectProductivity = true;
            lineItemView.InactivityReasonId = null;
            lineItemView.IsCommentNeededForReview = false;
            lineItemView.DeclaredBy = currentEvent.DeclaredBy;
            lineItemView.CreatedAt = currentEvent.CreatedAt;
        });
        
        ProjectEvent<InactivityCreatedEvent>((lineItemView, currentEvent) => 
        { 
            lineItemView.ShiftId = currentEvent.ShiftId;
            lineItemView.WashingMachineCode = currentEvent.WashingMachineCode;
            lineItemView.LineCode = currentEvent.LineCode;
            lineItemView.StartDate = currentEvent.StartDate;
            lineItemView.EndDate = currentEvent.EndDate;
            lineItemView.Type = (LineItemType)currentEvent.Type;
            lineItemView.IsPlanned = currentEvent.IsPlanned;
            lineItemView.InactivityReasonId = currentEvent.InactivityReasonId;
            lineItemView.IsCommentNeededForReview = currentEvent.IsCommentNeededForReview;
            lineItemView.AffectProductivity = currentEvent.AffectProductivity;
            lineItemView.DeclaredBy = currentEvent.DeclaredBy;
            lineItemView.CreatedAt = currentEvent.CreatedAt;
        });
        
        ProjectEvent<KitCommentChangedEvent>((lineItemView, currentEvent) => 
        { 
            lineItemView.Comment = currentEvent.Comment;
            lineItemView.UpdatedAt = currentEvent.UpdatedAt;
            lineItemView.UpdatedBy = currentEvent.UpdatedBy;
        });
        
        ProjectEvent<InactivityCommentAddedEvent>((lineItemView, currentEvent) => 
        { 
            lineItemView.Comment = currentEvent.Comment;
            lineItemView.UpdatedAt = currentEvent.UpdatedAt;
            lineItemView.UpdatedBy = currentEvent.UpdatedBy;
        });
        
        
        ProjectEvent<InactivityTypeChangedEvent>((lineItemView, currentEvent) => 
        { 
            lineItemView.InactivityReasonId = currentEvent.InactivityReasonId;
            lineItemView.IsCommentNeededForReview = currentEvent.IsCommentNeededForReview;
            lineItemView.AffectProductivity = currentEvent.AffectProductivity;
            lineItemView.Type = (LineItemType)currentEvent.Type;
            lineItemView.UpdatedAt = currentEvent.UpdatedAt;
            lineItemView.UpdatedBy = currentEvent.UpdatedBy;
        });
        
        ProjectEvent<InactivityTimeIntervalChangedEvent>((lineItemView, currentEvent) => 
        { 
            lineItemView.StartDate = currentEvent.StartDate;
            lineItemView.EndDate = currentEvent.EndDate;
            lineItemView.UpdatedAt = currentEvent.UpdatedAt;
            lineItemView.UpdatedBy = currentEvent.UpdatedBy;
        });
        
        ProjectEvent<KitTimeIntervalChangedEvent>((lineItemView, currentEvent) => 
        { 
            lineItemView.StartDate = currentEvent.StartDate;
            lineItemView.EndDate = currentEvent.EndDate;
            lineItemView.RealTimeKitDuration = currentEvent.RealTimeKitDuration;
            lineItemView.KitEfficiency = currentEvent.KitEfficiency;
            lineItemView.UpdatedAt = currentEvent.UpdatedAt;
            lineItemView.UpdatedBy = currentEvent.UpdatedBy;
        });
        
        ProjectEvent<InactivityUnplannedEvent>((lineItemView, currentEvent) => 
        { 
            lineItemView.IsPlanned = false;
            lineItemView.UpdatedAt = currentEvent.UpdatedAt;
            lineItemView.UpdatedBy = currentEvent.UpdatedBy;
        });
    }
}
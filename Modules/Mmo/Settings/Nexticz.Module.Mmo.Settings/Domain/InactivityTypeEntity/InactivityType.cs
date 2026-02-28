using Nexticz.Module.Mmo.Settings.Domain.InactivityTypeEntity.Events;
using Nexticz.Module.Mmo.SharedKernel.DomainCore;

namespace Nexticz.Module.Mmo.Settings.Domain.InactivityTypeEntity;

public class InactivityType : Entity
{
    public string Name { get; private set; }
    public bool AffectProductivity { get; private set; }
    public bool IsCommentNeededForReview { get; private set; }
    
    // We need private constructor due to Marten deserialization
    private InactivityType() {}

    public InactivityType(
        string name,
        bool affectProductivity,
        bool isCommentNeededForReview,
        Guid? id = null) : base(id ?? Guid.NewGuid())
    {
        Name = name;
        AffectProductivity = affectProductivity;
        IsCommentNeededForReview = isCommentNeededForReview;
    }

    public void Apply(InactivityTypeCreatedEvent @event)
    {
        Name = @event.Name;
        AffectProductivity = @event.AffectProductivity;
        IsCommentNeededForReview = @event.IsCommentNeededForReview;
    }
    
    public void Apply(InactivityTypeUpdatedEvent @event)
    {
        Name = @event.Name;
        AffectProductivity = @event.AffectProductivity;
        IsCommentNeededForReview = @event.IsCommentNeededForReview;
    }
}
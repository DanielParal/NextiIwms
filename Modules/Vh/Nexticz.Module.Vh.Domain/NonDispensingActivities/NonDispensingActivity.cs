using Nexticz.Module.Vh.Domain.ActivityCategories;
using Nexticz.Module.Vh.Domain.Centers;
using Nexticz.Module.Vh.Domain.LoadingActionsNdas;

namespace Nexticz.Module.Vh.Domain.NonDispensingActivities;

public class NonDispensingActivity
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public required string ActivityIdentifier { get; set; }
    public required string Name { get; set; }
    public required bool RequireNote { get; set; }
    public string? Note { get; set; }
    public required string Unit { get; set; }
    public required decimal Coefficient { get; set; }
    public TimeOnly? CutOff { get; set; }
    public required Guid CenterId { get; set; }
    public Center? Center { get; set; }
    public required Guid ActivityCategoryId { get; set; }
    public ActivityCategory? ActivityCategory { get; set; }
    public ICollection<LoadingActionsNda>? LoadingActionsNdas { get; set; }
}
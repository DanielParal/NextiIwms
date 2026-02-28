using Nexticz.Module.Vh.Contracts.ActivityCategories;
using Nexticz.Module.Vh.Contracts.Centers;

namespace Nexticz.Module.Vh.Contracts.NonDispensingActivities;

public class NonDispensingActivityResponse
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
    public required Guid ActivityCategoryId { get; set; }
}
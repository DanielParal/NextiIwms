using Nexticz.Module.Vh.Contracts.WorkerShifts;

namespace Nexticz.Module.Vh.Contracts.ActivityCategories;

public class ActivityCategoryResponse
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public required string Color { get; set; }
    public required ActivityType ActivityType { get; set; }
}
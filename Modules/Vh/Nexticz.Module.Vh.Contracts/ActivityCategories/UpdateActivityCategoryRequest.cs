using Nexticz.Module.Vh.Contracts.WorkerShifts;

namespace Nexticz.Module.Vh.Contracts.ActivityCategories;

public class UpdateActivityCategoryRequest
{
    public required string Name { get; set; }
    public required string Color { get; set; }
    public required ActivityType ActivityType { get; set; }
}
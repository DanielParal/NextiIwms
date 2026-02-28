using Nexticz.Module.Vh.Domain.NonDispensingActivities;
using Nexticz.Module.Vh.Domain.SystemActivities;
using Nexticz.Module.Vh.Domain.WorkerShifts;

namespace Nexticz.Module.Vh.Domain.ActivityCategories;

public class ActivityCategory
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public required string Name { get; set; }

    public required ActivityType ActivityType { get; set; }
    public required string Color { get; set; }
    public ICollection<NonDispensingActivity>? NonDispensingActivities { get; set; }
    public ICollection<SystemActivity>? SystemActivities { get; set; }
}
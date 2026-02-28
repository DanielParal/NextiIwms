using Nexticz.Module.Vh.Domain.Depositors;
using Nexticz.Module.Vh.Domain.DepositorsGroups;
using Nexticz.Module.Vh.Domain.NonDispensingActivities;
using Nexticz.Module.Vh.Domain.VhUsers;
using Nexticz.Module.Vh.Domain.Workers;

namespace Nexticz.Module.Vh.Domain.Centers;

public class Center
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public required string Name { get; set; }
    public required string Code { get; set; }
    public required bool ShowDashboardSalaryData { get; set; }
    public ICollection<DepositorsGroup>? DepositorsGroups { get; set; }
    public ICollection<Depositor>? Depositors { get; set; }
    public ICollection<Worker>? Workers { get; set; }
    public ICollection<NonDispensingActivity>? NonDispensingActivities { get; set; }
    public ICollection<VhUser>? VhUsers { get; set; }
}
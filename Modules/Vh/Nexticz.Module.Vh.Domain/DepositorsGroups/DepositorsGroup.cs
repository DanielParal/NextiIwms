using Nexticz.Module.Vh.Domain.Centers;
using Nexticz.Module.Vh.Domain.Depositors;
using Nexticz.Module.Vh.Domain.SystemActivities;

namespace Nexticz.Module.Vh.Domain.DepositorsGroups;

public class DepositorsGroup
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public required string Name { get; set; }
    public required string Code { get; set; }
    public required Guid CenterId { get; set; }
    public Center? Center { get; set; }
    public ICollection<Depositor>? Depositors { get; set; }
    public ICollection<SystemActivity>? SystemActivities { get; set; }
}
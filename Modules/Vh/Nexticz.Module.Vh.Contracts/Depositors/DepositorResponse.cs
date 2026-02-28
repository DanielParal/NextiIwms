using Nexticz.Module.Vh.Contracts.Centers;
using Nexticz.Module.Vh.Contracts.DepositorsGroups;

namespace Nexticz.Module.Vh.Contracts.Depositors;

public class DepositorResponse
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public required string Name { get; set; }
    public required string Code { get; set; }
    public required Guid CenterId { get; set; }
    public CenterResponse? Center { get; set; }
    public required Guid DepositorsGroupId { get; set; }
    public DepositorsGroupResponse? DepositorsGroup { get; set; }
}
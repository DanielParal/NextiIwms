using Nexticz.Module.Vh.Domain.Centers;
using Nexticz.Module.Vh.Domain.DepositorsGroups;

namespace Nexticz.Module.Vh.Domain.Depositors;

public class Depositor
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public required string Name { get; set; }
    public required string Code { get; set; }
    public required Guid CenterId { get; set; }
    public Center? Center { get; set; }
    public required Guid DepositorsGroupId { get; set; }
    public DepositorsGroup? DepositorsGroup { get; set; }
}
namespace Nexticz.Module.Vh.Contracts.DepositorsGroups;

public class UpdateDepositorsGroupRequest
{
    public required string Name { get; set; }
    public required string Code { get; set; }
    public required Guid CenterId { get; set; }
}
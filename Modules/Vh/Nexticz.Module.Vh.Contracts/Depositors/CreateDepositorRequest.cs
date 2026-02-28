namespace Nexticz.Module.Vh.Contracts.Depositors;

public class CreateDepositorRequest
{
    public required string Name { get; set; }
    public required string Code { get; set; }
    public required Guid CenterId { get; set; }
    public required Guid DepositorsGroupId { get; set; }
}
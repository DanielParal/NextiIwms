using Nexticz.Module.Vh.Contracts.Centers;

namespace Nexticz.Module.Vh.Contracts.DepositorsGroups;

public class DepositorsGroupResponse
{
    public required Guid Id { get; set; }
    public required string Name { get; set; }
    public required string? Code { get; set; }
    public required Guid CenterId { get; set; }
    public CenterResponse? Center { get; set; }
}
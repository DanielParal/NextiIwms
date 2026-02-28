namespace Nexticz.Module.Vh.Contracts.Centers;

public class UpdateCenterRequest
{
    public required string Name { get; set; }
    public required string Code { get; set; }
    public required bool ShowDashboardSalaryData { get; set; }
}
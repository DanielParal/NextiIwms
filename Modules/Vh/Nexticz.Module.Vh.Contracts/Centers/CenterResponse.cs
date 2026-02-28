namespace Nexticz.Module.Vh.Contracts.Centers;

public class CenterResponse
{
    public required Guid Id { get; set; }
    public required string Name { get; set; }
    public required string Code { get; set; }
    public required bool ShowDashboardSalaryData { get; set; }
}
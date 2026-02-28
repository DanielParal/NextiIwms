namespace Nexticz.Module.Mmo.Reporting.Application.Shifts.Models;

internal class ShiftSummary
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public DateTimeOffset StartDate { get; set; }
    public DateTimeOffset EndDate { get; set; }
    public TimeSpan AdjustmentTime => TimeSpan.FromTicks(MachineSummaries.Sum(x => x.AdjustmentTime.Ticks));
    public TimeSpan ShutdownTime => TimeSpan.FromTicks(MachineSummaries.Sum(x => x.ShutdownTime.Ticks));
    public TimeSpan DowntimeTime => TimeSpan.FromTicks(MachineSummaries.Sum(x => x.DowntimeTime.Ticks));
    public double Efficiency => MachineSummaries.Sum(x => x.AvailableFundTimeToCalculateEfficiency.TotalSeconds) == 0 ? 0 
        : (MachineSummaries.Sum(x => x.RealWashingTime.TotalSeconds) / MachineSummaries.Sum(x => x.AvailableFundTimeToCalculateEfficiency.TotalSeconds)) * 100;
    public List<ShiftWashingMachineSummary> MachineSummaries { get; set; } = [];
}
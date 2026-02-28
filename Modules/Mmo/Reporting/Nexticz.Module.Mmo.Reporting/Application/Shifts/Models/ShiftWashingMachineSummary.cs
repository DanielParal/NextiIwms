namespace Nexticz.Module.Mmo.Reporting.Application.Shifts.Models;

internal class ShiftWashingMachineSummary
{
    public string Code { get; set; }
    public TimeSpan AdjustmentTime => TimeSpan.FromTicks(LineSummaries.Sum(x => x.AdjustmentTime.Ticks));
    public TimeSpan ShutdownTime => TimeSpan.FromTicks(LineSummaries.Sum(x => x.ShutdownTime.Ticks));
    public TimeSpan DowntimeTime => TimeSpan.FromTicks(LineSummaries.Sum(x => x.DowntimeTime.Ticks));
    public TimeSpan RealWashingTime => TimeSpan.FromTicks(LineSummaries.Sum(x => x.RealWashingTime.Ticks));
    public TimeSpan AvailableFundTimeToCalculateEfficiency => TimeSpan.FromTicks(LineSummaries.Sum(x => x.AvailableFundTimeToCalculateEfficiency.Ticks));
    public double Efficiency => LineSummaries.Sum(x => x.AvailableFundTimeToCalculateEfficiency.TotalSeconds) == 0 ? 0 
        : (LineSummaries.Sum(x => x.RealWashingTime.TotalSeconds) / LineSummaries.Sum(x => x.AvailableFundTimeToCalculateEfficiency.TotalSeconds)) * 100;
    public List<ShiftWashingLineSummary> LineSummaries { get; set; } = [];
}
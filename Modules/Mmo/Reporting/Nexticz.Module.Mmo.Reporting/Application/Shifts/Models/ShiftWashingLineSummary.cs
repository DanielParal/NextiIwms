namespace Nexticz.Module.Mmo.Reporting.Application.Shifts.Models;

internal class ShiftWashingLineSummary
{
    public string Code { get; set; }
    public TimeSpan LineTotalTime { get; set; }
    public TimeSpan AdjustmentTime { get; set; }
    public TimeSpan ImpactProductivityShutdownTime { get; set; }
    public TimeSpan NonImpactProductivityShutdownTime { get; set; }
    public TimeSpan ShutdownTime => ImpactProductivityShutdownTime + NonImpactProductivityShutdownTime;
    public TimeSpan ImpactProductivityDowntimeTime { get; set; }
    public TimeSpan NonImpactProductivityDowntimeTime { get; set; }
    public TimeSpan DowntimeTime => ImpactProductivityDowntimeTime + NonImpactProductivityDowntimeTime;
    public TimeSpan RealWashingTime { get; set; }
    public TimeSpan OptimalWashingTime { get; set; }
    public TimeSpan AvailableFundTimeToCalculateEfficiency => LineTotalTime - AdjustmentTime - NonImpactProductivityShutdownTime - NonImpactProductivityDowntimeTime;
    public double Efficiency => AvailableFundTimeToCalculateEfficiency == TimeSpan.Zero ? 0 
        : (RealWashingTime.TotalSeconds / AvailableFundTimeToCalculateEfficiency.TotalSeconds) * 100;
}
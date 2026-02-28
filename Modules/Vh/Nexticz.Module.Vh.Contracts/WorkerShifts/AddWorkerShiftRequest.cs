namespace Nexticz.Module.Vh.Contracts.WorkerShifts;

public class AddWorkerShiftRequest
{
    public required DateTime Start { get; set; }
    public required string WorkerCode { get; set; }
    public required string CenterCode { get; set; }
    public required string ActivityCode { get; set; }
    public string? DepositorCode { get; set; }
    public string? DepositorGroupCode { get; set; }
    public required string ActivitySystemType { get; set; }
    public ActivityType ActivityType { get; set; }
    public ActivitySource ActivitySource { get; set; }
    public string? Note { get; set; }
    public required string ActivityName { get; set; }
    public TimeOnly? ActivityCutOff { get; set; }
    public string? Unit { get; set; }
    public decimal Coefficient { get; set; }
}
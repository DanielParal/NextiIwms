namespace Nexticz.Module.Vh.Domain.ShiftMasterChanges;

public class ShiftMasterChange
{
    public ShiftMasterChange(string user, ShiftMasterActivityType shiftMasterActivityType, string workerCode,
        string centerCode, Guid workerShiftId)
    {
        User = user;
        Date = DateTime.Now;
        ShiftMasterActivityType = shiftMasterActivityType;
        WorkerCode = workerCode;
        CenterCode = centerCode;
        WorkerShiftId = workerShiftId;
    }

    public Guid Id { get; set; }
    public string User { get; set; }
    public DateTime Date { get; set; }
    public ShiftMasterActivityType ShiftMasterActivityType { get; set; }
    public string WorkerCode { get; set; }
    public string CenterCode { get; set; }
    public Guid WorkerShiftId { get; set; }
    public List<WorkerShiftChange> WorkerShiftChanges { get; set; } = [];
    public List<WorkerShiftActivityChange> WorkerShiftActivityChanges { get; set; } = [];
}

public enum ShiftMasterActivityType
{
    AddWorkerShift,
    AddActivity,
    EndWorkerShift,
    ApproveWorkerShift,
    DisApproveWorkerShift
}
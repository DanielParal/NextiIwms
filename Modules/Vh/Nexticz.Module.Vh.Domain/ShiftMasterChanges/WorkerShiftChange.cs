using Nexticz.Lib.Shared.Helpers;

namespace Nexticz.Module.Vh.Domain.ShiftMasterChanges;

public class WorkerShiftChange
{
    private WorkerShiftChange()
    {
    }

    public WorkerShiftChange(ContextChangeType contextChangeType, Guid workerShiftId, DateTime start, DateTime? end,
        string workerCode, string workerCenterCode, bool approved)
    {
        WorkerShiftId = workerShiftId;
        ContextChangeType = contextChangeType;
        Start = start;
        End = end;
        WorkerCode = workerCode;
        WorkerCenterCode = workerCenterCode;
        Approved = approved;
    }

    public Guid Id { get; set; }
    public Guid WorkerShiftId { get; set; }
    public ContextChangeType ContextChangeType { get; set; }
    public DateTime Start { get; set; }
    public DateTime? End { get; set; }
    public string WorkerCode { get; set; }
    public string WorkerCenterCode { get; set; }
    public bool Approved { get; private set; }
}
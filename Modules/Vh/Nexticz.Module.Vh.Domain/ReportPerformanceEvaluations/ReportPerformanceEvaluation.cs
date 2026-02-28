using Nexticz.Module.Vh.Domain.WorkerShifts;

namespace Nexticz.Module.Vh.Domain.ReportPerformanceEvaluations;

public class ReportPerformanceEvaluation
{
    private ReportPerformanceEvaluation()
    {
    }

    public ReportPerformanceEvaluation(
        DateOnly date,
        Guid workerShiftId,
        DateTime workerShiftStart,
        DateTime workerShiftEnd,
        string centerCode,
        string workerCode,
        string workerName,
        decimal durationTime,
        decimal score,
        decimal scoreMyStock,
        decimal scoreIwms,
        decimal scoreSag,
        decimal scoreNonProductive,
        decimal salary,
        decimal zone
        )
    {
        Date = date;
        WorkerShiftId = workerShiftId;
        WorkerShiftStart = workerShiftStart;
        WorkerShiftEnd = workerShiftEnd;
        CenterCode = centerCode;
        WorkerCode = workerCode;
        WorkerName = workerName;
        DurationTime = durationTime;
        Score = score;
        ScoreMyStock = scoreMyStock;
        ScoreIwms = scoreIwms;
        ScoreSag = scoreSag;
        ScoreNonProductive = scoreNonProductive;
        Salary = salary;
        Zone = zone;
    }

    public Guid Id { get; init; }
    public DateOnly Date { get; private set; }
    public Guid WorkerShiftId { get; private set; }
    public DateTime WorkerShiftStart { get; private set; }
    public DateTime WorkerShiftEnd { get; private set; }
    public string CenterCode { get; private set; }
    public string WorkerCode { get; private set; }
    public string WorkerName { get; private set; }
    public decimal DurationTime { get; private set; }
    public decimal Score { get; private set; }
    public decimal ScoreMyStock { get; private set; }
    public decimal ScoreIwms { get; private set; }
    public decimal ScoreSag { get; private set; }
    public decimal ScoreNonProductive { get; private set; }
    public decimal Salary { get; private set; }
    public decimal Zone { get; private set; }

    public WorkerShift WorkerShift { get; set; }
}
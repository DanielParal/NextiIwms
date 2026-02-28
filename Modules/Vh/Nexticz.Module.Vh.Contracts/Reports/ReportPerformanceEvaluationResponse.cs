namespace Nexticz.Module.Vh.Contracts.Reports;

public class ReportPerformanceEvaluationResponse
{
    public required ReportPerformanceEvaluationWorker ReportPerformanceEvaluationWorkerSalary { get; set; }
    public required ReportPerformanceEvaluationWorker ReportPerformanceEvaluationTimeDuration { get; set; }
    public required ReportPerformanceEvaluationWorker ReportPerformanceEvaluationScoreSum { get; set; }
    public required ReportPerformanceEvaluationWorker ReportPerformanceEvaluationScoreMyStock { get; set; }
    public required ReportPerformanceEvaluationWorker ReportPerformanceEvaluationScoreIwms { get; set; }
    public required ReportPerformanceEvaluationWorker ReportPerformanceEvaluationScoreSaq { get; set; }
    public required ReportPerformanceEvaluationWorker ReportPerformanceEvaluationScoreNonProductive { get; set; }
    public required List<int> Days { get; set; }
    public required List<string> Workers { get; set; }
    public required string ExcelFile { get; set; }
}

public class ReportPerformanceEvaluationWorker
{
    public required List<ReportPerformanceEvaluationWorkerRow> WorkerRow { get; set; }
    public required decimal Sum { get; set; }
}

public class ReportPerformanceEvaluationWorkerRow
{
    public required string WorkerCode { get; set; }
    public required string Name { get; set; }
    public required List<ReportPerformanceEvaluationWorkerRowDay> WorkerRowDaysValues { get; set; }
    public required decimal Sum { get; set; }
}

public class ReportPerformanceEvaluationWorkerRowDay
{
    public required int Day { get; set; }
    public required decimal Value { get; set; }
}
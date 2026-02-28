using ClosedXML.Excel;
using ErrorOr;
using MediatR;
using Nexticz.Module.Vh.Contracts.Reports;
using Nexticz.Module.Vh.Domain.ReportPerformanceEvaluations;
using Nexticz.Module.Vh.Application.Common.Interfaces;

namespace Nexticz.Module.Vh.Application.Reports.Queries.GetReportPerformanceEvaluation;

public class GetReportPerformanceEvaluationQueryHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<GetReportPerformanceEvaluationQuery, ErrorOr<ReportPerformanceEvaluationResponse>>
{
    private ReportPerformanceEvaluationWorker ReportPerformanceEvaluationWorkerSalary { get; } =
        new() { WorkerRow = [], Sum = 0 };

    private ReportPerformanceEvaluationWorker ReportPerformanceEvaluationTimeDuration { get; } =
        new() { WorkerRow = [], Sum = 0 };

    private ReportPerformanceEvaluationWorker ReportPerformanceEvaluationScoreSum { get; } =
        new() { WorkerRow = [], Sum = 0 };

    private ReportPerformanceEvaluationWorker ReportPerformanceEvaluationScoreMyStock { get; } =
        new() { WorkerRow = [], Sum = 0 };

    private ReportPerformanceEvaluationWorker ReportPerformanceEvaluationScoreIwms { get; } =
        new() { WorkerRow = [], Sum = 0 };

    private ReportPerformanceEvaluationWorker ReportPerformanceEvaluationScoreSaq { get; } =
        new() { WorkerRow = [], Sum = 0 };

    private ReportPerformanceEvaluationWorker ReportPerformanceEvaluationScoreNonProductive { get; } =
        new() { WorkerRow = [], Sum = 0 };

    public async Task<ErrorOr<ReportPerformanceEvaluationResponse>> Handle(GetReportPerformanceEvaluationQuery query,
        CancellationToken cancellationToken)
    {
        var year = query.FilteringParams.FromYear ?? DateTime.Now.Year;
        var month = query.FilteringParams.FromMonth ?? DateTime.Now.Month;
        var maxDays = DateTime.DaysInMonth(year, month);

        var days = Enumerable.Range(1, maxDays).ToList();

        var performanceEvaluationData =
            (await unitOfWork.ReportPerformanceEvaluationRepository.GetReportPerformanceEvaluationAsync(
                query.FilteringParams,
                cancellationToken)).data
            .OfType<ReportPerformanceEvaluation>()
            .GroupBy(x => x.WorkerCode)
            .OrderBy(x => x.Key)
            .ToList();

        var workersCodes = performanceEvaluationData.Select(x => x.Key).ToList();

        foreach (var reportPerformanceEvaluation in performanceEvaluationData)
        {
            var workerShiftsByDays = reportPerformanceEvaluation
                .GroupBy(x => x.Date)
                .OrderBy(x => x.Key);

            ReportPerformanceEvaluationWorkerRow workerSalaryRow = new()
            {
                WorkerCode = reportPerformanceEvaluation.Key,
                Name = reportPerformanceEvaluation.First(x => x.WorkerCode == reportPerformanceEvaluation.Key)
                    .WorkerName,
                WorkerRowDaysValues = [],
                Sum = 0
            };
            ReportPerformanceEvaluationWorkerRow workerTimeDurationRow = new()
            {
                WorkerCode = reportPerformanceEvaluation.Key,
                Name = reportPerformanceEvaluation.First(x => x.WorkerCode == reportPerformanceEvaluation.Key)
                    .WorkerName,
                WorkerRowDaysValues = [],
                Sum = 0
            };
            ReportPerformanceEvaluationWorkerRow workerSScoreSumRow = new()
            {
                WorkerCode = reportPerformanceEvaluation.Key,
                Name = reportPerformanceEvaluation.First(x => x.WorkerCode == reportPerformanceEvaluation.Key)
                    .WorkerName,
                WorkerRowDaysValues = [],
                Sum = 0
            };
            ReportPerformanceEvaluationWorkerRow workerScoreMyStockRow = new()
            {
                WorkerCode = reportPerformanceEvaluation.Key,
                Name = reportPerformanceEvaluation.First(x => x.WorkerCode == reportPerformanceEvaluation.Key)
                    .WorkerName,
                WorkerRowDaysValues = [],
                Sum = 0
            };
            ReportPerformanceEvaluationWorkerRow workerScoreIwmsRow = new()
            {
                WorkerCode = reportPerformanceEvaluation.Key,
                Name = reportPerformanceEvaluation.First(x => x.WorkerCode == reportPerformanceEvaluation.Key)
                    .WorkerName,
                WorkerRowDaysValues = [],
                Sum = 0
            };
            ReportPerformanceEvaluationWorkerRow workerScoreSaqRow = new()
            {
                WorkerCode = reportPerformanceEvaluation.Key,
                Name = reportPerformanceEvaluation.First(x => x.WorkerCode == reportPerformanceEvaluation.Key)
                    .WorkerName,
                WorkerRowDaysValues = [],
                Sum = 0
            };
            ReportPerformanceEvaluationWorkerRow workerNonProductiveRow = new()
            {
                WorkerCode = reportPerformanceEvaluation.Key,
                Name = reportPerformanceEvaluation.First(x => x.WorkerCode == reportPerformanceEvaluation.Key)
                    .WorkerName,
                WorkerRowDaysValues = [],
                Sum = 0
            };

            foreach (var workerShiftsByDay in workerShiftsByDays)
            {
                var dayNumber = workerShiftsByDay.Key.Day;

                // AddDayToDaysArrayIfNotContains(dayNumber, days);

                workerSalaryRow.WorkerRowDaysValues.Add(new ReportPerformanceEvaluationWorkerRowDay
                {
                    Day = dayNumber,
                    Value = workerShiftsByDay.Sum(x => Math.Round(x.Salary))
                });

                workerTimeDurationRow.WorkerRowDaysValues.Add(new ReportPerformanceEvaluationWorkerRowDay
                {
                    Day = dayNumber,
                    Value = workerShiftsByDay.Sum(x => Math.Round(x.DurationTime))
                });

                workerSScoreSumRow.WorkerRowDaysValues.Add(new ReportPerformanceEvaluationWorkerRowDay
                {
                    Day = dayNumber,
                    Value = workerShiftsByDay.Sum(x => Math.Round(x.Score))
                });

                workerScoreMyStockRow.WorkerRowDaysValues.Add(new ReportPerformanceEvaluationWorkerRowDay
                {
                    Day = dayNumber,
                    Value = workerShiftsByDay.Sum(x => Math.Round(x.ScoreMyStock))
                });

                workerScoreIwmsRow.WorkerRowDaysValues.Add(new ReportPerformanceEvaluationWorkerRowDay
                {
                    Day = dayNumber,
                    Value = workerShiftsByDay.Sum(x => Math.Round(x.ScoreIwms))
                });

                workerScoreSaqRow.WorkerRowDaysValues.Add(new ReportPerformanceEvaluationWorkerRowDay
                {
                    Day = dayNumber,
                    Value = workerShiftsByDay.Sum(x => Math.Round(x.ScoreSag))
                });

                workerNonProductiveRow.WorkerRowDaysValues.Add(new ReportPerformanceEvaluationWorkerRowDay
                {
                    Day = dayNumber,
                    Value = workerShiftsByDay.Sum(x => Math.Round(x.ScoreNonProductive))
                });
            }

            workerSalaryRow.Sum = workerSalaryRow.WorkerRowDaysValues.Sum(x => Math.Round(x.Value));
            workerTimeDurationRow.Sum = workerTimeDurationRow.WorkerRowDaysValues.Sum(x => Math.Round(x.Value));
            workerSScoreSumRow.Sum = workerSScoreSumRow.WorkerRowDaysValues.Sum(x => Math.Round(x.Value));
            workerScoreMyStockRow.Sum = workerScoreMyStockRow.WorkerRowDaysValues.Sum(x => Math.Round(x.Value));
            workerScoreIwmsRow.Sum = workerScoreIwmsRow.WorkerRowDaysValues.Sum(x => Math.Round(x.Value));
            workerScoreSaqRow.Sum = workerScoreSaqRow.WorkerRowDaysValues.Sum(x => Math.Round(x.Value));
            workerNonProductiveRow.Sum = workerNonProductiveRow.WorkerRowDaysValues.Sum(x => Math.Round(x.Value));

            ReportPerformanceEvaluationWorkerSalary.WorkerRow.Add(workerSalaryRow);
            ReportPerformanceEvaluationTimeDuration.WorkerRow.Add(workerTimeDurationRow);
            ReportPerformanceEvaluationScoreSum.WorkerRow.Add(workerSScoreSumRow);
            ReportPerformanceEvaluationScoreMyStock.WorkerRow.Add(workerScoreMyStockRow);
            ReportPerformanceEvaluationScoreIwms.WorkerRow.Add(workerScoreIwmsRow);
            ReportPerformanceEvaluationScoreSaq.WorkerRow.Add(workerScoreSaqRow);
            ReportPerformanceEvaluationScoreNonProductive.WorkerRow.Add(workerNonProductiveRow);
        }

        ReportPerformanceEvaluationWorkerSalary.Sum =
            ReportPerformanceEvaluationWorkerSalary.WorkerRow.Sum(x => Math.Round(x.Sum));
        ReportPerformanceEvaluationTimeDuration.Sum =
            ReportPerformanceEvaluationTimeDuration.WorkerRow.Sum(x => Math.Round(x.Sum));
        ReportPerformanceEvaluationScoreSum.Sum =
            ReportPerformanceEvaluationScoreSum.WorkerRow.Sum(x => Math.Round(x.Sum));
        ReportPerformanceEvaluationScoreMyStock.Sum =
            ReportPerformanceEvaluationScoreMyStock.WorkerRow.Sum(x => Math.Round(x.Sum));
        ReportPerformanceEvaluationScoreIwms.Sum =
            ReportPerformanceEvaluationScoreIwms.WorkerRow.Sum(x => Math.Round(x.Sum));
        ReportPerformanceEvaluationScoreSaq.Sum =
            ReportPerformanceEvaluationScoreSaq.WorkerRow.Sum(x => Math.Round(x.Sum));
        ReportPerformanceEvaluationScoreNonProductive.Sum =
            ReportPerformanceEvaluationScoreNonProductive.WorkerRow.Sum(x => Math.Round(x.Sum));

        days.Sort();

        var report = new ReportPerformanceEvaluationResponse
        {
            Days = days,
            Workers = workersCodes,
            ReportPerformanceEvaluationWorkerSalary = ReportPerformanceEvaluationWorkerSalary,
            ReportPerformanceEvaluationTimeDuration = ReportPerformanceEvaluationTimeDuration,
            ReportPerformanceEvaluationScoreSum = ReportPerformanceEvaluationScoreSum,
            ReportPerformanceEvaluationScoreMyStock = ReportPerformanceEvaluationScoreMyStock,
            ReportPerformanceEvaluationScoreIwms = ReportPerformanceEvaluationScoreIwms,
            ReportPerformanceEvaluationScoreSaq = ReportPerformanceEvaluationScoreSaq,
            ReportPerformanceEvaluationScoreNonProductive = ReportPerformanceEvaluationScoreNonProductive,
            ExcelFile = ""
        };

        report.ReportPerformanceEvaluationWorkerSalary.WorkerRow = report.ReportPerformanceEvaluationWorkerSalary
            .WorkerRow.OrderBy(x => x.Name).ToList();
        report.ReportPerformanceEvaluationTimeDuration.WorkerRow = report.ReportPerformanceEvaluationTimeDuration
            .WorkerRow.OrderBy(x => x.Name).ToList();
        report.ReportPerformanceEvaluationScoreSum.WorkerRow =
            report.ReportPerformanceEvaluationScoreSum.WorkerRow.OrderBy(x => x.Name).ToList();
        report.ReportPerformanceEvaluationScoreMyStock.WorkerRow = report.ReportPerformanceEvaluationScoreMyStock
            .WorkerRow.OrderBy(x => x.Name).ToList();
        report.ReportPerformanceEvaluationScoreIwms.WorkerRow =
            report.ReportPerformanceEvaluationScoreIwms.WorkerRow.OrderBy(x => x.Name).ToList();
        report.ReportPerformanceEvaluationScoreSaq.WorkerRow =
            report.ReportPerformanceEvaluationScoreSaq.WorkerRow.OrderBy(x => x.Name).ToList();
        report.ReportPerformanceEvaluationScoreNonProductive.WorkerRow = report
            .ReportPerformanceEvaluationScoreNonProductive.WorkerRow.OrderBy(x => x.Name).ToList();
        report.ExcelFile = CreateExcelFile(report);

        return report;
    }

    private static string CreateExcelFile(ReportPerformanceEvaluationResponse report)
    {
        using var workbook = new XLWorkbook();
        var worksheetWorkerSalary = workbook.Worksheets.Add("Hodnocení zaměstnance");
        var worksheetTimeDuration = workbook.Worksheets.Add("Odpracováno minut");
        var worksheetScoreSum = workbook.Worksheets.Add("Skóre celkem");
        var worksheetScoreMyStock = workbook.Worksheets.Add("Skóre myStock");
        var worksheetScoreIwms = workbook.Worksheets.Add("Skóre iWMS");
        var worksheetScoreSag = workbook.Worksheets.Add("Skóre SAG");
        var worksheetScoreNonProductive = workbook.Worksheets.Add("Neproduktivní činnost");

        GenerateWorksheetData(worksheetWorkerSalary, report.ReportPerformanceEvaluationWorkerSalary, report.Days,
            report.Workers);
        GenerateWorksheetData(worksheetTimeDuration, report.ReportPerformanceEvaluationTimeDuration, report.Days,
            report.Workers);
        GenerateWorksheetData(worksheetScoreSum, report.ReportPerformanceEvaluationScoreSum, report.Days,
            report.Workers);
        GenerateWorksheetData(worksheetScoreMyStock, report.ReportPerformanceEvaluationScoreMyStock, report.Days,
            report.Workers);
        GenerateWorksheetData(worksheetScoreIwms, report.ReportPerformanceEvaluationScoreIwms, report.Days,
            report.Workers);
        GenerateWorksheetData(worksheetScoreSag, report.ReportPerformanceEvaluationScoreSaq, report.Days,
            report.Workers);
        GenerateWorksheetData(worksheetScoreNonProductive, report.ReportPerformanceEvaluationScoreNonProductive,
            report.Days, report.Workers);

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        var excelBytes = stream.ToArray();

        return Convert.ToBase64String(excelBytes);
        ;
    }

    private static void GenerateWorksheetData(IXLWorksheet worksheet, ReportPerformanceEvaluationWorker reportRows,
        List<int> reportDays, List<string> reportWorkers)
    {
        GenerateExportHeader(worksheet, reportDays);
        GenerateExportRow(worksheet, reportRows, reportDays, reportWorkers);
    }

    private static void GenerateExportHeader(IXLWorksheet worksheet, List<int> reportDays)
    {
        worksheet.Cell(1, 1).Value = "Jméno";
        for (var i = 1; i <= reportDays.Count; i++) worksheet.Cell(1, i + 1).Value = i;
        worksheet.Cell(1, reportDays.Count + 2).Value = "Celkem";
    }

    private static void GenerateExportRow(IXLWorksheet worksheet, ReportPerformanceEvaluationWorker reportRows,
        List<int> reportDays, List<string> reportWorkers)
    {
        var rowIndex = 2;
        foreach (var reportWorker in reportWorkers)
        {
            worksheet.Cell(rowIndex, 1).Value = GetWorkerName(reportWorker, reportRows);
            GenerateExportRowValues(worksheet, reportDays, reportWorker, reportRows, rowIndex);

            rowIndex++;
        }

        worksheet.Cell(rowIndex, reportDays.Count + 2).Value = reportRows.Sum;
    }

    private static void GenerateExportRowValues(IXLWorksheet worksheet, List<int> reportDays, string reportWorker,
        ReportPerformanceEvaluationWorker reportRows, int rowIndex)
    {
        for (var i = 1; i <= reportDays.Count; i++)
            worksheet.Cell(rowIndex, i + 1).Value = GetWorkerDayValue(reportWorker, i, reportRows);
        worksheet.Cell(rowIndex, reportDays.Count + 2).Value = GetWorkerRowSum(reportWorker, reportRows);
    }

    private static string GetWorkerName(string reportWorker, ReportPerformanceEvaluationWorker reportRows)
    {
        return reportRows.WorkerRow.Find(x => x.WorkerCode == reportWorker)!.Name;
    }

    private static decimal GetWorkerRowSum(string reportWorker, ReportPerformanceEvaluationWorker reportRows)
    {
        return reportRows.WorkerRow.Find(x => x.WorkerCode == reportWorker)!.Sum;
    }

    private static decimal? GetWorkerDayValue(string reportWorker, decimal day,
        ReportPerformanceEvaluationWorker reportRows)
    {
        var workerRow = reportRows.WorkerRow.Find(x => x.WorkerCode == reportWorker)!;

        var workerRowDay = workerRow.WorkerRowDaysValues.Find(x => x.Day == day);

        return workerRowDay is not null
            ? workerRow.WorkerRowDaysValues.Find(x => x.Day == day)!.Value
            : null;
    }
}
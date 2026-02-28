using ErrorOr;
using MediatR;
using Nexticz.Module.Vh.Contracts.DepositorsGroups;
using Nexticz.Module.Vh.Contracts.Reports;
using Nexticz.Module.Vh.Domain.Centers;
using Nexticz.Module.Vh.Domain.DepositorsGroups;
using Nexticz.Module.Vh.Domain.WorkerShifts;
using Nexticz.Module.Vh.Application.Common.Interfaces;
using Nexticz.Module.Vh.Application.DepositorsGroups.Common.Models;
using Nexticz.Module.Vh.Application.WorkerShifts.Common.Models;

namespace Nexticz.Module.Vh.Application.Reports.Queries.GetReportWorkerShiftPerformance;

public class GetReportWorkerShiftPerformanceQueryHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<GetReportWorkerShiftPerformanceQuery, ErrorOr<ReportWorkerShiftsPerformanceResponse>>
{
    public async Task<ErrorOr<ReportWorkerShiftsPerformanceResponse>> Handle(GetReportWorkerShiftPerformanceQuery query,
        CancellationToken cancellationToken)
    {
        var center =
            await unitOfWork.CentersRepository.GetCenterByCodeAsync(query.PerformanceReportsRequest.CenterCode,
                cancellationToken);

        if (center is null)
            return CenterErrors.CenterWithIdDoesnotExist;

        var depositorGroupsFilter = $"[\"{nameof(DepositorsGroup.CenterId)}\",\"=\",\"{center.Id}\"]";
        var depositorGroups = (await unitOfWork.DepositorsGroupsRepository.GetDepositorsGroupsAsync(
                new DepositorsGroupsFilteringParams { Filter = depositorGroupsFilter }, cancellationToken)).data
            .OfType<DepositorsGroupResponse>().ToList();

        depositorGroups = depositorGroups.DistinctBy(x => x.Name).ToList();

        depositorGroups.Add(new DepositorsGroupResponse
            { Code = null, Name = "Nesystémové činnosti", CenterId = Guid.Empty, Id = Guid.Empty });

        var workerShiftsFilter = $"[\"{nameof(WorkerShift.Approved)}\",\"=\",\"true\"]";
        var workerShifts =
            (await unitOfWork.WorkerShiftsRepository.GetWorkerShiftsAsync(
                new WorkerShiftsFilteringParams
                {
                    Filter = workerShiftsFilter, ActivityCenter = query.PerformanceReportsRequest.CenterCode,
                    FromMonth = query.PerformanceReportsRequest.Month, FromYear = query.PerformanceReportsRequest.Year
                }, cancellationToken))
            .data.OfType<WorkerShift>().ToList();

        var workerShiftsActivities = workerShifts.SelectMany(x => x.Activities).ToList();

        workerShiftsActivities = workerShiftsActivities
            .Where(x => x.CenterCode == query.PerformanceReportsRequest.CenterCode).ToList();

        var workerShiftsActivitiesWithoutBreaks =
            workerShiftsActivities.Where(x => x.ActivityType != ActivityType.Break).ToList();

        var report = new ReportWorkerShiftsPerformanceResponse
        {
            ReportWorkerShiftsPerformanceDays = [],
            ReportWorkerShiftsPerformanceSum = new ReportWorkerShiftsPerformanceSum
            {
                PerformanceSum = 0, ScoreSum = 0, DurationTimeSum = 0, ReportActivitiesPerformanceDepositorGroups = []
            },
            DepositorsGroups = depositorGroups
        };

        CalculateReportRows(report, workerShiftsActivitiesWithoutBreaks);
        CalculateReportSum(report);
        RemoveFromReportEmptyDays(report);

        return report;
    }

    private void CalculateReportRows(ReportWorkerShiftsPerformanceResponse report,
        List<WorkerShiftActivity> workerShiftsActivitiesWithoutBreaks)
    {
        for (var i = 1; i <= 31; i++)
        {
            var reportDay = new ReportWorkerShiftsPerformanceDay
            {
                Day = i, ReportActivitiesPerformanceDepositorGroups = [], DurationTimeSum = 0, ScoreSum = 0,
                PerformanceSum = 0
            };
            CalculateDepositorsRow(reportDay, workerShiftsActivitiesWithoutBreaks, report.DepositorsGroups);
            CalculateRowSum(reportDay);
            report.ReportWorkerShiftsPerformanceDays.Add(reportDay);
        }
    }

    private void CalculateDepositorsRow(ReportWorkerShiftsPerformanceDay reportDay,
        List<WorkerShiftActivity> workerShiftsActivitiesWithoutBreaks, List<DepositorsGroupResponse> depositorGroups)
    {
        var depositorsEcolabCount = depositorGroups.Count(x => x.Code != null && x.Code.StartsWith("ECOLAB"));

        foreach (var depositorsGroupResponse in depositorGroups)
        {
            var depositorRow = new ReportActivitiesPerformanceDepositorGroup
                { DepositorGroupCode = depositorsGroupResponse.Code, DurationTime = 0, Score = 0, Performance = 0 };
            CalculateDepositorRow(reportDay.Day, depositorRow, workerShiftsActivitiesWithoutBreaks,
                depositorsGroupResponse, depositorsEcolabCount);
            reportDay.ReportActivitiesPerformanceDepositorGroups.Add(depositorRow);
        }
    }

    private void CalculateDepositorRow(int day, ReportActivitiesPerformanceDepositorGroup depositorRow,
        List<WorkerShiftActivity> workerShiftsActivitiesWithoutBreaks, DepositorsGroupResponse depositorsGroupResponse,
        int depositorsEcolabCount)
    {
        if (depositorRow.DepositorGroupCode is not null && depositorRow.DepositorGroupCode.StartsWith("ECOLAB") &&
            depositorsEcolabCount == 1)
        {
            string[] ecolabCodes = { "ECOLAB_C", "ECOLAB_OST_C" };

            depositorRow.DurationTime = workerShiftsActivitiesWithoutBreaks
                .Where(x => ecolabCodes.Contains(x.DepositorGroupCode) && x.End!.Value.Day == day)
                .Sum(x => x.DurationMinutes);

            depositorRow.Score = workerShiftsActivitiesWithoutBreaks
                .Where(x => ecolabCodes.Contains(x.DepositorGroupCode) && x.End!.Value.Day == day)
                .Sum(x => x.Score);

            depositorRow.Performance = CalculatePerformance(depositorRow.Score, depositorRow.DurationTime);

            return;
        }

        depositorRow.DurationTime = workerShiftsActivitiesWithoutBreaks
            .Where(x => x.DepositorGroupCode == depositorsGroupResponse.Code && x.End!.Value.Day == day)
            .Sum(x => x.DurationMinutes);

        depositorRow.Score = workerShiftsActivitiesWithoutBreaks
            .Where(x => x.DepositorGroupCode == depositorsGroupResponse.Code && x.End!.Value.Day == day)
            .Sum(x => x.Score);

        depositorRow.Performance = CalculatePerformance(depositorRow.Score, depositorRow.DurationTime);
    }

    private void CalculateRowSum(ReportWorkerShiftsPerformanceDay reportDay)
    {
        reportDay.DurationTimeSum = reportDay.ReportActivitiesPerformanceDepositorGroups.Sum(x => x.DurationTime);
        reportDay.ScoreSum = reportDay.ReportActivitiesPerformanceDepositorGroups.Sum(x => x.Score);
        reportDay.PerformanceSum = CalculatePerformance(reportDay.ScoreSum, reportDay.DurationTimeSum);
    }

    private void CalculateReportSum(ReportWorkerShiftsPerformanceResponse report)
    {
        var reportActivitiesPerformanceDepositorGroups = report.ReportWorkerShiftsPerformanceDays.SelectMany(x => x
            .ReportActivitiesPerformanceDepositorGroups).ToList();

        foreach (var depositorsGroupResponse in report.DepositorsGroups)
        {
            var durationTime = reportActivitiesPerformanceDepositorGroups
                .Where(y => y.DepositorGroupCode == depositorsGroupResponse.Code)
                .Sum(z => z.DurationTime);

            var score = reportActivitiesPerformanceDepositorGroups
                .Where(y => y.DepositorGroupCode == depositorsGroupResponse.Code)
                .Sum(z => z.Score);

            var depositorSum = new ReportActivitiesPerformanceDepositorGroup
            {
                DepositorGroupCode = depositorsGroupResponse.Code,
                DurationTime = durationTime,
                Score = score,
                Performance = CalculatePerformance(score, durationTime)
            };

            report.ReportWorkerShiftsPerformanceSum.ReportActivitiesPerformanceDepositorGroups.Add(depositorSum);
        }

        report.ReportWorkerShiftsPerformanceSum.DurationTimeSum = report.ReportWorkerShiftsPerformanceSum
            .ReportActivitiesPerformanceDepositorGroups
            .Sum(x => x.DurationTime);

        report.ReportWorkerShiftsPerformanceSum.ScoreSum = report.ReportWorkerShiftsPerformanceSum
            .ReportActivitiesPerformanceDepositorGroups
            .Sum(x => x.Score);

        report.ReportWorkerShiftsPerformanceSum.PerformanceSum = CalculatePerformance(
            report.ReportWorkerShiftsPerformanceSum.ScoreSum, report.ReportWorkerShiftsPerformanceSum.DurationTimeSum);
    }

    private decimal CalculatePerformance(decimal score, decimal durationTime)
    {
        if (durationTime == 0)
            return 0;

        return Math.Round(score * 100 / durationTime, 2);
    }

    private void RemoveFromReportEmptyDays(ReportWorkerShiftsPerformanceResponse report)
    {
        report.ReportWorkerShiftsPerformanceDays =
            report.ReportWorkerShiftsPerformanceDays.Where(x => x.DurationTimeSum != 0).ToList();
    }
}
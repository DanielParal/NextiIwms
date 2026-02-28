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

namespace Nexticz.Module.Vh.Application.Reports.Queries.GetReportActivitiesPerformance;

public class GetReportActivitiesPerformanceQueryHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<GetReportActivitiesPerformanceQuery, ErrorOr<ReportActivitiesPerformanceResponse>>
{
    public async Task<ErrorOr<ReportActivitiesPerformanceResponse>> Handle(GetReportActivitiesPerformanceQuery query,
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
            { Code = null, Name = "Neznámá", CenterId = Guid.Empty, Id = Guid.Empty });

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

        // foreach (var workerShiftsActivity in workerShiftsActivities)
        //     Console.WriteLine("-" + workerShiftsActivity.ActivitySystemType + workerShiftsActivity.ActivitiesCount +
        //                       workerShiftsActivity.ActivityCode);

        var workerShiftsActivitiesWithoutBreaks =
            workerShiftsActivities.Where(x => x.ActivityType != ActivityType.Break).ToList();

        var report = new ReportActivitiesPerformanceResponse
        {
            ReportWorkerShiftsPerformanceDays = [],
            ReportWorkerShiftsPerformanceSum = new ReportActivitiesPerformanceSum
            {
                ActivitiesCount = 0, PerformanceSum = 0, SystemActivitiesHours = 0, WorkersFundHours = 0,
                NonDispensingActivitiesHours = 0, ReportActivitiesPerformanceDepositorGroups = []
            },
            DepositorsGroups = depositorGroups
        };

        CalculateReportRows(report, workerShiftsActivitiesWithoutBreaks);
        CalculateReportSum(report);
        RemoveFromReportEmptyDays(report);

        return report;
    }

    private void CalculateReportRows(ReportActivitiesPerformanceResponse report,
        List<WorkerShiftActivity> workerShiftsActivitiesWithoutBreaks)
    {
        for (var i = 1; i <= 31; i++)
        {
            var reportDay = new ReportActivitiesPerformanceDay
            {
                Day = i, ReportActivitiesPerformanceDepositorGroupsActivities = [], ActivitiesCount = 0,
                PerformanceSum = 0, SystemActivitiesHours = 0, WorkersFundHours = 0,
                NonDispensingActivitiesHours = 0
            };
            CalculateDepositorsRow(reportDay, workerShiftsActivitiesWithoutBreaks, report.DepositorsGroups);
            CalculateRowSum(reportDay, workerShiftsActivitiesWithoutBreaks);
            report.ReportWorkerShiftsPerformanceDays.Add(reportDay);
        }
    }

    private void CalculateDepositorsRow(ReportActivitiesPerformanceDay reportDay,
        List<WorkerShiftActivity> workerShiftsActivitiesWithoutBreaks, List<DepositorsGroupResponse> depositorGroups)
    {
        var depositorsEcolabCount = depositorGroups.Count(x => x.Code != null && x.Code.StartsWith("ECOLAB"));
        
        foreach (var depositorsGroupResponse in depositorGroups)
        {
            var depositorRow = new ReportActivitiesPerformanceDepositorGroupActivities
                { DepositorGroupCode = depositorsGroupResponse.Code, ActivitiesCount = 0 };
            CalculateDepositorRow(reportDay.Day, depositorRow, workerShiftsActivitiesWithoutBreaks,
                depositorsGroupResponse, depositorsEcolabCount);
            reportDay.ReportActivitiesPerformanceDepositorGroupsActivities.Add(depositorRow);
        }
    }

    private void CalculateDepositorRow(int day, ReportActivitiesPerformanceDepositorGroupActivities depositorRow,
        List<WorkerShiftActivity> workerShiftsActivitiesWithoutBreaks, DepositorsGroupResponse depositorsGroupResponse, int depositorsEcolabCount)
    {
        if (depositorRow.DepositorGroupCode is not null && depositorRow.DepositorGroupCode.StartsWith("ECOLAB") && depositorsEcolabCount == 1)
        {
            string[] ecolabCodes = {"ECOLAB_C","ECOLAB_OST_C"};
            
            depositorRow.ActivitiesCount = workerShiftsActivitiesWithoutBreaks
                .Where(x => ecolabCodes.Contains(x.DepositorGroupCode) && x.End!.Value.Day == day &&
                            x.ActivityCode == "VYCHYSTAVANI")
                .Sum(x => x.ActivitiesCount);

            return;
        }

        if (depositorsGroupResponse.Code == null)
        {
            depositorRow.ActivitiesCount = 0;
            return;
        }
        
        depositorRow.ActivitiesCount = workerShiftsActivitiesWithoutBreaks
            .Where(x => x.DepositorGroupCode == depositorsGroupResponse.Code && x.End!.Value.Day == day &&
                        x.ActivityCode == "VYCHYSTAVANI")
            .Sum(x => x.ActivitiesCount);
    }

    private void CalculateRowSum(ReportActivitiesPerformanceDay reportDay,
        List<WorkerShiftActivity> workerShiftsActivitiesWithoutBreaks)
    {
        reportDay.ActivitiesCount =
            reportDay.ReportActivitiesPerformanceDepositorGroupsActivities.Sum(x => x.ActivitiesCount);
        reportDay.WorkersFundHours = Math.Round(workerShiftsActivitiesWithoutBreaks
            .Where(x => x.End!.Value.Day == reportDay.Day)
            .Sum(y => y.DurationMinutes) / 60, 2);
        reportDay.NonDispensingActivitiesHours = Math.Round(workerShiftsActivitiesWithoutBreaks
            .Where(x => x.ActivitySystemType == "Nevýdejová" && x.End!.Value.Day == reportDay.Day)
            .Sum(y => y.DurationMinutes) / 60, 2);
        reportDay.SystemActivitiesHours = Math.Round(workerShiftsActivitiesWithoutBreaks
            .Where(x => x.ActivitySystemType == "Výdejová" && x.End!.Value.Day == reportDay.Day)
            .Sum(y => y.DurationMinutes) / 60, 2);
        reportDay.PerformanceSum = CalculatePerformance(reportDay.ActivitiesCount, reportDay.SystemActivitiesHours);
    }

    private void CalculateReportSum(ReportActivitiesPerformanceResponse report)
    {
        var reportActivitiesPerformanceDepositorGroups = report.ReportWorkerShiftsPerformanceDays.SelectMany(x => x
            .ReportActivitiesPerformanceDepositorGroupsActivities).ToList();

        foreach (var depositorsGroupResponse in report.DepositorsGroups)
        {
            var activitiesCount = reportActivitiesPerformanceDepositorGroups
                .Where(y => y.DepositorGroupCode == depositorsGroupResponse.Code)
                .Sum(z => z.ActivitiesCount);

            var depositorSum = new ReportActivitiesPerformanceDepositorGroupActivities
            {
                DepositorGroupCode = depositorsGroupResponse.Code,
                ActivitiesCount = activitiesCount
            };

            report.ReportWorkerShiftsPerformanceSum.ReportActivitiesPerformanceDepositorGroups.Add(depositorSum);
        }

        report.ReportWorkerShiftsPerformanceSum.ActivitiesCount = report.ReportWorkerShiftsPerformanceDays
            .Sum(x => x.ActivitiesCount);

        report.ReportWorkerShiftsPerformanceSum.WorkersFundHours = report.ReportWorkerShiftsPerformanceDays
            .Sum(x => x.WorkersFundHours);
        report.ReportWorkerShiftsPerformanceSum.NonDispensingActivitiesHours = report.ReportWorkerShiftsPerformanceDays
            .Sum(x => x.NonDispensingActivitiesHours);
        report.ReportWorkerShiftsPerformanceSum.SystemActivitiesHours = report.ReportWorkerShiftsPerformanceDays
            .Sum(x => x.SystemActivitiesHours);
        report.ReportWorkerShiftsPerformanceSum.PerformanceSum = CalculatePerformance(
            report.ReportWorkerShiftsPerformanceSum.ActivitiesCount,
            report.ReportWorkerShiftsPerformanceSum.SystemActivitiesHours);
    }

    private decimal CalculatePerformance(decimal activitiesCount, decimal systemActivitiesHours)
    {
        if (systemActivitiesHours == 0)
            return 0;

        return Math.Round(activitiesCount / systemActivitiesHours, 2);
    }

    private void RemoveFromReportEmptyDays(ReportActivitiesPerformanceResponse report)
    {
        report.ReportWorkerShiftsPerformanceDays =
            report.ReportWorkerShiftsPerformanceDays.Where(x => x.ActivitiesCount != 0 || x.WorkersFundHours != 0)
                .ToList();
    }
}
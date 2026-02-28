using ErrorOr;
using MediatR;
using Nexticz.Module.Vh.Contracts.BandRewards;
using Nexticz.Module.Vh.Contracts.Reports;
using Nexticz.Module.Vh.Contracts.Workers;
using Nexticz.Module.Vh.Domain.WorkerShifts;
using Nexticz.Module.Vh.Application.BandRewards.Common.Models;
using Nexticz.Module.Vh.Application.Common.Interfaces;
using Nexticz.Module.Vh.Application.Workers.Common.Models;
using Nexticz.Module.Vh.Application.WorkerShifts.Common.Models;

namespace Nexticz.Module.Vh.Application.Reports.Queries.GetDashboard;

public class GetDashboardQueryHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<GetDashboardQuery, ErrorOr<DashboardResponse>>
{
    public async Task<ErrorOr<DashboardResponse>> Handle(GetDashboardQuery query, CancellationToken cancellationToken)
    {
        var reportGenerationTime = DateTime.Now;
        var fromDate = DateTime.Now.Date;
        var toDate = fromDate.AddDays(1);
        var filter = new WorkerShiftsFilteringParams
        {
            ActivityCenter = query.FilteringParams.CenterCode, ActivitiesStart = fromDate, ActivitiesEnd = toDate,
            Filter = query.FilteringParams.Filter
        };
        var workerShifts = (await unitOfWork.WorkerShiftsRepository.GetWorkerShiftsAsync(filter, cancellationToken))
            .data.OfType<WorkerShift>().ToList();
        var bandRewards =
            (await unitOfWork.BandRewardsRepository.GetBandRewardsAsync(new BandRewardsFilteringParams(),
                cancellationToken)).data.OfType<BandRewardResponse>().ToList();
        var workers =
            (await unitOfWork.WorkersRepository.GetWorkersAsync(new WorkersFilteringParams(), cancellationToken)).data
            .OfType<WorkerResponse>().ToList();
        var centerShowSalaryData =
            (await unitOfWork.CentersRepository.GetCenterByCodeAsync(query.FilteringParams.CenterCode,
                cancellationToken))?.ShowDashboardSalaryData;

        var dashboard = new DashboardResponse
        {
            DashboardRows = [],
            DashboardGeneratedTime = TimeOnly.FromDateTime(reportGenerationTime),
            ShowSalaryData = centerShowSalaryData ?? false
        };

        foreach (var workerShift in workerShifts)
        {
            var worker = workers
                .FirstOrDefault(x => x.CodeWms == workerShift.WorkerCode);

            workerShift.Activities.ToList().ForEach(x =>
            {
                if (x.End is null) x.SetEnd(reportGenerationTime);
            });

            var unknownTimeSum = workerShift.Activities.Where(x => x.ActivityType == ActivityType.Unknown)
                .Sum(x => x.DurationMinutes);

            var durationTimeSum = workerShift.Activities.Where(x => x.ActivityType != ActivityType.Break)
                .Sum(x => x.DurationMinutes);
            var durationTimeTimespan = TimeSpan.FromMinutes((double)durationTimeSum);

            var scoreSum = workerShift.Activities.Sum(x => x.Score);
            var averageScore = scoreSum != 0 && durationTimeSum != 0
                ? Math.Floor(scoreSum / (durationTimeSum / 60) * 10) / 10
                : 0;
            var zone = bandRewards.FirstOrDefault(x => x.MinValue <= averageScore && x.MaxValue > averageScore);

            var row = new DashboardRow
            {
                WorkerName = worker is not null ? worker.Surname + " " + worker.Name : "",
                WorkerShiftStart = TimeOnly.FromDateTime(workerShift.Start),
                UnknownActivityTime = unknownTimeSum,
                MyStockScore = workerShift.Activities.Where(x => x.ActivityType == ActivityType.PaidMyStock)
                    .Sum(x => x.Score),
                DynamicsScore = workerShift.Activities.Where(x => x.ActivityType == ActivityType.PaidDynamics)
                    .Sum(x => x.Score),
                IwmsScore =
                    workerShift.Activities.Where(x => x.ActivityType == ActivityType.PaidIwms).Sum(x => x.Score),
                NonProductiveScore = workerShift.Activities.Where(x => x.ActivityType == ActivityType.NonProductive)
                    .Sum(x => x.Score),
                ScoreSum = scoreSum,
                TimeSum = durationTimeTimespan.ToString(),
                AverageScore = averageScore,
                ZoneNumber = zone?.BandNumber ?? 0,
                ZoneReward = zone?.Reward ?? 0,
                ActualSalary = durationTimeSum / 60 * zone?.Reward ?? 0
            };

            dashboard.DashboardRows.Add(row);

            dashboard.DashboardRows = dashboard.DashboardRows.OrderByDescending(x => x.AverageScore).ToList();
        }

        return dashboard;
    }
}
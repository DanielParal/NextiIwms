using ErrorOr;
using MediatR;
using Nexticz.Module.Vh.Contracts.ActivityCategories;
using Nexticz.Module.Vh.Contracts.BandRewards;
using Nexticz.Module.Vh.Contracts.DepositorsGroups;
using Nexticz.Module.Vh.Contracts.Reports;
using Nexticz.Module.Vh.Domain.WorkerShifts;
using Nexticz.Module.Vh.Application.ActivityCategories.Common.Models;
using Nexticz.Module.Vh.Application.BandRewards.Common.Models;
using Nexticz.Module.Vh.Application.Common.Interfaces;
using Nexticz.Module.Vh.Application.DepositorsGroups.Common.Models;

namespace Nexticz.Module.Vh.Application.Reports.Queries.GetDashboardPda;

public class GetDashboardPdaQueryHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<GetDashboardPdaQuery, ErrorOr<DashboardPdaResponse>>
{
    private DashboardPdaResponse DashboardPdaResponse { get; } = new()
        { PaidTypes = [], ScoreSum = 0, ActualSalary = 0, AverageScore = 0, ZoneNumber = 0, ShowSalaryData = false };

    public async Task<ErrorOr<DashboardPdaResponse>> Handle(GetDashboardPdaQuery query,
        CancellationToken cancellationToken)
    {
        var reportGenerationTime = DateTime.Now;
        var workerShift =
            await unitOfWork.WorkerShiftsRepository.GetWorkerShiftByIdAsync(query.DashboardPdaRequest.WorkerShiftId,
                cancellationToken);

        if (workerShift is null)
            return WorkerShiftErrors.WorkerShiftWithIdDoesNotExist;

        var bandRewards =
            (await unitOfWork.BandRewardsRepository.GetBandRewardsAsync(new BandRewardsFilteringParams(),
                cancellationToken)).data.OfType<BandRewardResponse>().ToList();

        var centerShowSalaryData =
            (await unitOfWork.CentersRepository.GetCenterByCodeAsync(workerShift.WorkerCenterCode,
                cancellationToken))?.ShowDashboardSalaryData;

        DashboardPdaResponse.ShowSalaryData = centerShowSalaryData ?? false;

        workerShift.Activities.ToList().ForEach(x =>
        {
            if (x.End is null) x.SetEnd(reportGenerationTime);
        });

        var activityCategories =
            (await unitOfWork.ActivityCategoriesRepository.GetActivityCategoriesAsync(
                new ActivityCategoriesFilteringParams(), cancellationToken)).data.OfType<ActivityCategoryResponse>()
            .ToList();

        var depositorGroups =
            (await unitOfWork.DepositorsGroupsRepository.GetDepositorsGroupsAsync(
                new DepositorsGroupsFilteringParams(), cancellationToken)).data.OfType<DepositorsGroupResponse>()
            .ToList();

        var activityTypes = workerShift.Activities.Where(x => x.ActivityType != ActivityType.Break)
            .GroupBy(x => x.ActivityType).ToList();

        var myStockActivities = activityTypes.Where(x => x.Key == ActivityType.PaidMyStock).ToList();
        var iwmsActivities = activityTypes.Where(x => x.Key == ActivityType.PaidIwms);
        var nonProductiveActivities = activityTypes.Where(x => x.Key == ActivityType.NonProductive);
        var unknownActivities = activityTypes.Where(x => x.Key == ActivityType.Unknown);
        var dynamicsActivities = activityTypes.Where(x => x.Key == ActivityType.PaidDynamics);

        var myStockGroupedActivities = myStockActivities.Select(x => new
        {
            PaidType = x.Key,
            DepositorGroups = x.GroupBy(a => a.DepositorGroupCode)
                .Select(dg => new
                {
                    Name = dg.Key,
                    Activities = dg.GroupBy(dga => dga.ActivityName).ToList()
                }).ToList()
        }).ToList();

        var myStockPaidType = new DashboardPdaPaidType
        {
            Name = "myStock",
            ActivityType = Contracts.WorkerShifts.ActivityType.PaidMyStock,
            Color = activityCategories.First(x => x.ActivityType == Contracts.WorkerShifts.ActivityType.PaidMyStock)
                .Color,
            ActivityTypes = []
        };

        if (myStockGroupedActivities.Count > 0)
            foreach (var depositorGroup in myStockGroupedActivities.First().DepositorGroups)
            {
                var activityType = new DashboardPdaActivityType
                {
                    Name = depositorGroups.FirstOrDefault(x => x.Code == depositorGroup.Name)?.Name ?? "???",
                    ScoreSum = depositorGroup.Activities.Sum(x => x.Sum(y => y.Score)),
                    DurationTimeSum = depositorGroup.Activities.Sum(x =>
                        x.Where(y => y.ActivityType != ActivityType.Break).Sum(z => z.DurationMinutes)),
                    DashboardPdaActivityTypesItems = []
                };

                foreach (var workerShiftActivities in depositorGroup.Activities)
                {
                    var unit = workerShiftActivities.First().Unit ?? "";

                    var count = unit == "min"
                        ? workerShiftActivities.Sum(x => x.DurationMinutes)
                        : workerShiftActivities.Sum(x => x.ActivitiesCount);

                    var item = new DashboardPdaActivityTypesItem
                    {
                        Name = workerShiftActivities.Key,
                        Count = count,
                        Unit = unit,
                        Coefficient = workerShiftActivities.First().Coefficient,
                        Score = workerShiftActivities.Sum(x => x.Score)
                    };
                    activityType.DashboardPdaActivityTypesItems.Add(item);
                }

                myStockPaidType.ActivityTypes.Add(activityType);
            }

        var dynamicsGroupedActivities = dynamicsActivities.Select(x => new
        {
            PaidType = x.Key,
            CenterGroups = x.GroupBy(a => a.CenterCode)
                .Select(dg => new
                {
                    Name = dg.Key,
                    Activities = dg.GroupBy(dga => dga.ActivityName).ToList()
                }).ToList()
        }).ToList();

        var dynamicsPaidType = new DashboardPdaPaidType
        {
            Name = "Dynamics",
            ActivityType = Contracts.WorkerShifts.ActivityType.PaidDynamics,
            Color = activityCategories.First(x => x.ActivityType == Contracts.WorkerShifts.ActivityType.PaidDynamics)
                .Color,
            ActivityTypes = []
        };

        if (dynamicsGroupedActivities.Count > 0)
            foreach (var depositorGroup in dynamicsGroupedActivities.First().CenterGroups)
            {
                var activityType = new DashboardPdaActivityType
                {
                    Name = depositorGroup.Name,
                    ScoreSum = depositorGroup.Activities.Sum(x => x.Sum(y => y.Score)),
                    DurationTimeSum = depositorGroup.Activities.Sum(x =>
                        x.Where(y => y.ActivityType != ActivityType.Break).Sum(z => z.DurationMinutes)),
                    DashboardPdaActivityTypesItems = []
                };

                foreach (var workerShiftActivities in depositorGroup.Activities)
                {
                    var unit = workerShiftActivities.First().Unit ?? "";

                    var count = unit == "min"
                        ? workerShiftActivities.Sum(x => x.DurationMinutes)
                        : workerShiftActivities.Sum(x => x.ActivitiesCount);

                    var item = new DashboardPdaActivityTypesItem
                    {
                        Name = workerShiftActivities.Key,
                        Count = count,
                        Unit = unit,
                        Coefficient = workerShiftActivities.First().Coefficient,
                        Score = workerShiftActivities.Sum(x => x.Score)
                    };
                    activityType.DashboardPdaActivityTypesItems.Add(item);
                }

                dynamicsPaidType.ActivityTypes.Add(activityType);
            }

        var iwmsGroupedActivities = iwmsActivities.Select(x => new
        {
            PaidType = x.Key,
            CenterGroups = x.GroupBy(a => a.CenterCode)
                .Select(dg => new
                {
                    Name = dg.Key,
                    Activities = dg.GroupBy(dga => dga.ActivityName).ToList()
                }).ToList()
        }).ToList();

        var iwmsPaidType = new DashboardPdaPaidType
        {
            Name = "iWMS",
            ActivityType = Contracts.WorkerShifts.ActivityType.PaidIwms,
            Color = activityCategories.First(x => x.ActivityType == Contracts.WorkerShifts.ActivityType.PaidIwms).Color,
            ActivityTypes = []
        };

        if (iwmsGroupedActivities.Count > 0)
            foreach (var depositorGroup in iwmsGroupedActivities.First().CenterGroups)
            {
                var activityType = new DashboardPdaActivityType
                {
                    Name = depositorGroup.Name,
                    ScoreSum = depositorGroup.Activities.Sum(x => x.Sum(y => y.Score)),
                    DurationTimeSum = depositorGroup.Activities.Sum(x =>
                        x.Where(y => y.ActivityType != ActivityType.Break).Sum(z => z.DurationMinutes)),
                    DashboardPdaActivityTypesItems = []
                };

                foreach (var workerShiftActivities in depositorGroup.Activities)
                {
                    var unit = workerShiftActivities.First().Unit ?? "";

                    var count = unit == "min"
                        ? workerShiftActivities.Sum(x => x.DurationMinutes)
                        : workerShiftActivities.Sum(x => x.ActivitiesCount);

                    var item = new DashboardPdaActivityTypesItem
                    {
                        Name = workerShiftActivities.Key,
                        Count = count,
                        Unit = unit,
                        Coefficient = workerShiftActivities.First().Coefficient,
                        Score = workerShiftActivities.Sum(x => x.Score)
                    };
                    activityType.DashboardPdaActivityTypesItems.Add(item);
                }

                iwmsPaidType.ActivityTypes.Add(activityType);
            }

        var nonProductiveGroupedActivities = nonProductiveActivities.Select(x => new
        {
            PaidType = x.Key,
            CenterGroups = x.GroupBy(a => a.CenterCode)
                .Select(dg => new
                {
                    Name = dg.Key,
                    Activities = dg.GroupBy(dga => dga.ActivityName).ToList()
                }).ToList()
        }).ToList();

        var nonProductivePaidType = new DashboardPdaPaidType
        {
            Name = "Neproduktivní",
            ActivityType = Contracts.WorkerShifts.ActivityType.NonProductive,
            Color = activityCategories.First(x => x.ActivityType == Contracts.WorkerShifts.ActivityType.NonProductive)
                .Color,
            ActivityTypes = []
        };

        if (nonProductiveGroupedActivities.Count > 0)
            foreach (var depositorGroup in nonProductiveGroupedActivities.First().CenterGroups)
            {
                var activityType = new DashboardPdaActivityType
                {
                    Name = depositorGroup.Name,
                    ScoreSum = depositorGroup.Activities.Sum(x => x.Sum(y => y.Score)),
                    DurationTimeSum = depositorGroup.Activities.Sum(x =>
                        x.Where(y => y.ActivityType != ActivityType.Break).Sum(z => z.DurationMinutes)),
                    DashboardPdaActivityTypesItems = []
                };

                foreach (var workerShiftActivities in depositorGroup.Activities)
                {
                    var unit = workerShiftActivities.First().Unit ?? "";

                    var count = unit == "min"
                        ? workerShiftActivities.Sum(x => x.DurationMinutes)
                        : workerShiftActivities.Sum(x => x.ActivitiesCount);

                    var item = new DashboardPdaActivityTypesItem
                    {
                        Name = workerShiftActivities.Key,
                        Count = count,
                        Unit = unit,
                        Coefficient = workerShiftActivities.First().Coefficient,
                        Score = workerShiftActivities.Sum(x => x.Score)
                    };
                    activityType.DashboardPdaActivityTypesItems.Add(item);
                }

                nonProductivePaidType.ActivityTypes.Add(activityType);
            }

        var unknownGroupedActivities = unknownActivities.Select(x => new
        {
            PaidType = x.Key,
            CenterGroups = x.GroupBy(a => a.CenterCode)
                .Select(dg => new
                {
                    Name = dg.Key,
                    Activities = dg.GroupBy(dga => dga.ActivityName).ToList()
                }).ToList()
        }).ToList();

        var unknownPaidType = new DashboardPdaPaidType
        {
            Name = "Neznámá",
            ActivityType = Contracts.WorkerShifts.ActivityType.Unknown,
            Color = activityCategories.First(x => x.ActivityType == Contracts.WorkerShifts.ActivityType.Unknown).Color,
            ActivityTypes = []
        };

        if (unknownGroupedActivities.Count > 0)
            foreach (var depositorGroup in unknownGroupedActivities.First().CenterGroups)
            {
                var activityType = new DashboardPdaActivityType
                {
                    Name = unknownPaidType.Name,
                    ScoreSum = depositorGroup.Activities.Sum(x => x.Sum(y => y.Score)),
                    DurationTimeSum = depositorGroup.Activities.Sum(x =>
                        x.Where(y => y.ActivityType != ActivityType.Break).Sum(z => z.DurationMinutes)),
                    DashboardPdaActivityTypesItems = []
                };

                foreach (var workerShiftActivities in depositorGroup.Activities)
                {
                    var unit = workerShiftActivities.First().Unit ?? "min";

                    var count = unit == "min"
                        ? workerShiftActivities.Sum(x => x.DurationMinutes)
                        : workerShiftActivities.Sum(x => x.ActivitiesCount);

                    var item = new DashboardPdaActivityTypesItem
                    {
                        Name = "Neznámá činnost",
                        Count = count,
                        Unit = unit,
                        Coefficient = workerShiftActivities.First().Coefficient,
                        Score = workerShiftActivities.Sum(x => x.Score)
                    };
                    activityType.DashboardPdaActivityTypesItems.Add(item);
                }

                unknownPaidType.ActivityTypes.Add(activityType);
            }

        DashboardPdaResponse.PaidTypes.Add(myStockPaidType);
        DashboardPdaResponse.PaidTypes.Add(iwmsPaidType);
        DashboardPdaResponse.PaidTypes.Add(dynamicsPaidType);
        DashboardPdaResponse.PaidTypes.Add(nonProductivePaidType);
        DashboardPdaResponse.PaidTypes.Add(unknownPaidType);

        var scoreSum = DashboardPdaResponse.PaidTypes
            .Sum(x => x.ActivityTypes.Sum(y => y.ScoreSum));

        var durationTimeSum = DashboardPdaResponse.PaidTypes
            .Sum(x => x.ActivityTypes.Sum(y => y.DurationTimeSum));

        var averageScore = scoreSum != 0 && durationTimeSum != 0
            ? Math.Floor(scoreSum / (durationTimeSum / 60) * 10) / 10
            : 0;

        var zone = bandRewards.FirstOrDefault(x => x.MinValue <= averageScore && x.MaxValue > averageScore);

        var actualSalary = durationTimeSum / 60 * zone?.Reward ?? 0;

        DashboardPdaResponse.ScoreSum = scoreSum;
        DashboardPdaResponse.AverageScore = averageScore;
        DashboardPdaResponse.ActualSalary = actualSalary;
        DashboardPdaResponse.ZoneNumber = zone?.BandNumber ?? 0;

        return DashboardPdaResponse;
    }
}
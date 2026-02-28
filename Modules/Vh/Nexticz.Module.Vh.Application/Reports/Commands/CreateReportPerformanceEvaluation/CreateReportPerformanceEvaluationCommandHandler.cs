using ErrorOr;
using MediatR;
using Nexticz.Module.Vh.Contracts.BandRewards;
using Nexticz.Module.Vh.Domain.ReportPerformanceEvaluations;
using Nexticz.Module.Vh.Domain.Workers;
using Nexticz.Module.Vh.Domain.WorkerShifts;
using Nexticz.Module.Vh.Application.BandRewards.Common.Models;
using Nexticz.Module.Vh.Application.Common.Interfaces;

namespace Nexticz.Module.Vh.Application.Reports.Commands.CreateReportPerformanceEvaluation;

public class CreateReportPerformanceEvaluationCommandHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<CreateReportPerformanceEvaluationCommand, ErrorOr<Created>>
{
    public async Task<ErrorOr<Created>> Handle(CreateReportPerformanceEvaluationCommand command,
        CancellationToken cancellationToken)
    {
        var workerShift =
            await unitOfWork.WorkerShiftsRepository.GetWorkerShiftByIdAsync(command.WorkerShiftId, cancellationToken);

        if (workerShift is null)
            return WorkerShiftErrors.WorkerShiftWithIdDoesNotExist;

        var bandRewards =
            (await unitOfWork.BandRewardsRepository
                .GetBandRewardsAsync(new BandRewardsFilteringParams(), cancellationToken)).data
            .OfType<BandRewardResponse>()
            .ToList();

        var worker =
            await unitOfWork.WorkersRepository.GetWorkerResponseBySlugAsync(workerShift.WorkerCode, cancellationToken);

        if (worker is null)
            return WorkerErrors.WorkerWithIdDoesnotExist;

        var activitiesWithoutBrakes = workerShift.Activities
            .Where(x => x.ActivityType != ActivityType.Break)
            .ToList();

        var durationTime = activitiesWithoutBrakes.Sum(x => x.DurationMinutes);
        var durationTimeInHours = durationTime / 60;
        var score = activitiesWithoutBrakes.Sum(x => x.Score);
        var scoreMyStock = activitiesWithoutBrakes
            .Where(x => x.ActivityType == ActivityType.PaidMyStock)
            .Sum(x => x.Score);
        var scoreIwms = activitiesWithoutBrakes
            .Where(x => x.ActivityType == ActivityType.PaidIwms)
            .Sum(x => x.Score);
        var scoreSag = activitiesWithoutBrakes
            .Where(x => x.ActivityType == ActivityType.PaidDynamics)
            .Sum(x => x.Score);
        var scoreNonProductive = activitiesWithoutBrakes
            .Where(x => x.ActivityType == ActivityType.NonProductive)
            .Sum(x => x.Score);
        var zone = score / durationTimeInHours;
        var salary = bandRewards
            .FirstOrDefault(x => x.MinValue <= zone && x.MaxValue > zone)
            ?.Reward * durationTimeInHours;

        var performanceEvaluation = new ReportPerformanceEvaluation(
            DateOnly.FromDateTime((DateTime)workerShift.End!),
            workerShift.Id,
            workerShift.Start,
            (DateTime)workerShift.End!,
            workerShift.WorkerCenterCode,
            workerShift.WorkerCode,
            worker.Surname + " " + worker.Name,
            durationTime,
            score,
            scoreMyStock,
            scoreIwms,
            scoreSag,
            scoreNonProductive,
            salary ?? 0,
            zone
        );

        unitOfWork.Add(performanceEvaluation);

        await unitOfWork.CompleteAsync(cancellationToken);

        return Result.Created;
    }
}
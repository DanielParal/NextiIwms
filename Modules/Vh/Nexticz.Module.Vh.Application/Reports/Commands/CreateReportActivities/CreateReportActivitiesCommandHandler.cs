using ErrorOr;
using MediatR;
using Nexticz.Module.Vh.Domain.ReportActivities;
using Nexticz.Module.Vh.Domain.WorkerShifts;
using Nexticz.Module.Vh.Application.Common.Interfaces;

namespace Nexticz.Module.Vh.Application.Reports.Commands.CreateReportActivities;

public class CreateReportActivitiesCommandHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<CreateReportActivitiesCommand, ErrorOr<Created>>
{
    public async Task<ErrorOr<Created>> Handle(CreateReportActivitiesCommand command,
        CancellationToken cancellationToken)
    {
        var workerShift =
            await unitOfWork.WorkerShiftsRepository.GetWorkerShiftByIdAsync(command.WorkerShiftId, cancellationToken);

        if (workerShift is null)
            return WorkerShiftErrors.WorkerShiftWithIdDoesNotExist;

        List<ReportActivity> reportActivities = [];

        var distinctActivities = workerShift.Activities.DistinctBy(x => new { x.ActivityCode, x.ActivityName, x.Note })
            .ToList();

        foreach (var workerShiftActivity in distinctActivities)
        {
            var durationTime = workerShift.Activities
                .Where(x => x.ActivityName == workerShiftActivity.ActivityName &&
                            x.CenterCode == workerShiftActivity.CenterCode && x.Note == workerShiftActivity.Note)
                .Sum(y => y.DurationMinutes);
            var activitiesCount = workerShift.Activities
                .Where(x => x.ActivityName == workerShiftActivity.ActivityName &&
                            x.CenterCode == workerShiftActivity.CenterCode && x.Note == workerShiftActivity.Note)
                .Sum(y => y.ActivitiesCount);
            var score = workerShift.Activities
                .Where(x => x.ActivityName == workerShiftActivity.ActivityName &&
                            x.CenterCode == workerShiftActivity.CenterCode && x.Note == workerShiftActivity.Note)
                .Sum(y => y.Score);
            var workerShiftPowerPercentage = score * 100 / durationTime;

            if (workerShiftPowerPercentage > 999)
                workerShiftPowerPercentage = 100;

            reportActivities.Add(new ReportActivity(
                DateOnly.FromDateTime(workerShiftActivity.Start),
                workerShiftActivity.WorkerShiftId,
                workerShift.Start,
                (DateTime)workerShift.End!,
                workerShift.WorkerCenterCode,
                workerShiftActivity.CenterCode,
                workerShiftActivity.DepositorCode,
                workerShiftActivity.DepositorGroupCode,
                workerShiftActivity.ActivitySystemType,
                workerShiftActivity.ActivitySource,
                workerShiftActivity.WorkerCode,
                workerShiftActivity.ActivityCode,
                durationTime,
                activitiesCount,
                workerShiftActivity.Coefficient,
                score,
                workerShiftPowerPercentage
            ) { Note = workerShiftActivity.Note });
        }

        unitOfWork.AddRange(reportActivities);

        await unitOfWork.CompleteAsync(cancellationToken);

        return Result.Created;
    }
}
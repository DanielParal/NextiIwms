using ErrorOr;
using MediatR;
using Nexticz.Module.Vh.Domain.ShiftMasterChanges;
using Nexticz.Module.Vh.Domain.WorkerShifts;
using Nexticz.Lib.Shared.Helpers;
using Nexticz.Lib.Shared.UserProviders;
using Nexticz.Module.Vh.Application.WorkerShifts.Common.Helpers;
using Interfaces_IUnitOfWork = Nexticz.Module.Vh.Application.Common.Interfaces.IUnitOfWork;
using IUnitOfWork = Nexticz.Module.Vh.Application.Common.Interfaces.IUnitOfWork;

namespace Nexticz.Module.Vh.Application.WorkerShifts.Commands.AddWorkerShift;

public class AddWorkerShiftCommandHandler(Interfaces_IUnitOfWork unitOfWork, ICurrentUserProvider currentUserProvider)
    : IRequestHandler<AddWorkerShiftCommand, ErrorOr<Created>>
{
    public async Task<ErrorOr<Created>> Handle(AddWorkerShiftCommand command, CancellationToken cancellationToken)
    {
        var activity = new WorkerShiftActivity(
            command.AddWorkerShiftRequest.Start,
            command.AddWorkerShiftRequest.WorkerCode,
            command.AddWorkerShiftRequest.CenterCode,
            command.AddWorkerShiftRequest.ActivityCode,
            command.AddWorkerShiftRequest.DepositorCode,
            command.AddWorkerShiftRequest.DepositorGroupCode,
            command.AddWorkerShiftRequest.ActivitySystemType,
            Enum.Parse<ActivityType>(command.AddWorkerShiftRequest.ActivityType.ToString()),
            Enum.Parse<ActivitySource>(command.AddWorkerShiftRequest.ActivitySource.ToString()))
        {
            Note = command.AddWorkerShiftRequest.Note,
            ActivityCutOff = command.AddWorkerShiftRequest.ActivityCutOff,
            Coefficient = command.AddWorkerShiftRequest.Coefficient,
            Unit = command.AddWorkerShiftRequest.Unit,
            ActivityName = command.AddWorkerShiftRequest.ActivityName
        };

        var worker =
            await unitOfWork.WorkersRepository.GetWorkerResponseBySlugAsync(command.AddWorkerShiftRequest.WorkerCode,
                cancellationToken);

        if (worker is null)
            return WorkerShiftErrors.AddWorkerShiftError;

        var center = await unitOfWork.CentersRepository.GetCenterByIdAsync(worker.CenterId, cancellationToken);

        if (center is null)
            return WorkerShiftErrors.AddWorkerShiftError;

        var nonDispensingActivity =
            await unitOfWork.NonDispensingActivitiesRepository.GetNonDispensingActivityResponseBySlugAsync(
                worker.ActivityAfterCutOffCode, cancellationToken);

        var activityName = nonDispensingActivity is not null
            ? nonDispensingActivity.Name
            : "";

        var workerShift = new WorkerShift(center.Code, activity)
        {
            ManualStart = true, ActivityAfterCutOffCode = worker.ActivityAfterCutOffCode,
            ActivityAfterCutOffName = activityName
        };

        workerShift.CheckLastNotEndedActivityCutOff();

        unitOfWork.Add(workerShift);

        var loggedItems = LogContextChanges.GetLoggedItems(unitOfWork);

        await WorkerShiftHelper.AddLoggedItemsToContext(unitOfWork, currentUserProvider, loggedItems,
            ShiftMasterActivityType.AddWorkerShift, activity.WorkerCode, activity.CenterCode, activity.Id,
            cancellationToken);

        await unitOfWork.CompleteAsync(cancellationToken);

        return Result.Created;
    }
}
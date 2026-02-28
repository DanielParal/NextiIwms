using ErrorOr;
using MediatR;
using Nexticz.Module.Vh.Domain.ShiftMasterChanges;
using Nexticz.Module.Vh.Domain.WorkerShifts;
using Nexticz.Lib.Shared.Helpers;
using Nexticz.Lib.Shared.UserProviders;
using Nexticz.Module.Vh.Application.WorkerShifts.Common.Helpers;
using Interfaces_IUnitOfWork = Nexticz.Module.Vh.Application.Common.Interfaces.IUnitOfWork;
using IUnitOfWork = Nexticz.Module.Vh.Application.Common.Interfaces.IUnitOfWork;

namespace Nexticz.Module.Vh.Application.WorkerShifts.Commands.EndWorkerShift;

public class EndWorkerShiftCommandHandler(Interfaces_IUnitOfWork unitOfWork, ICurrentUserProvider currentUserProvider)
    : IRequestHandler<EndWorkerShiftCommand, ErrorOr<Created>>
{
    public async Task<ErrorOr<Created>> Handle(EndWorkerShiftCommand command, CancellationToken cancellationToken)
    {
        var workerShift =
            await unitOfWork.WorkerShiftsRepository.GetWorkerShiftByIdAsync(
                command.Id, cancellationToken);

        if (workerShift is null)
            return WorkerShiftErrors.WorkerShiftWithIdDoesNotExist;

        var endTime = workerShift.Activities.Count == 1 && workerShift.Activities[0].Start == command.End
            ? command.End.AddMinutes(1)
            : command.End;

        workerShift.ManualEnd = true;

        var splittedActivities = workerShift.CloseWorkerShift(endTime);

        var loggedItems = LogContextChanges.GetLoggedItems(unitOfWork);

        var firstActivity = workerShift.Activities.First();

        await WorkerShiftHelper.AddLoggedItemsToContext(unitOfWork, currentUserProvider, loggedItems,
            ShiftMasterActivityType.EndWorkerShift, firstActivity.WorkerCode, firstActivity.CenterCode,
            firstActivity.Id, cancellationToken);

        await unitOfWork.CompleteAsync(cancellationToken);

        if (splittedActivities.Count <= 0)
            return Result.Created;

        var worker =
            await unitOfWork.WorkersRepository.GetWorkerResponseBySlugAsync(workerShift.WorkerCode,
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

        var newWorkerShift = new WorkerShift(center.Code, splittedActivities[0])
        {
            ActivityAfterCutOffCode = worker.ActivityAfterCutOffCode,
            ActivityAfterCutOffName = activityName
        };

        for (var i = 1; i < splittedActivities.Count; i++) newWorkerShift.AddActivity(splittedActivities[i]);
        unitOfWork.Add(newWorkerShift);

        if (newWorkerShift.Activities.Last().End is not null)
            newWorkerShift.End = newWorkerShift.Activities.Last().End;

        loggedItems = LogContextChanges.GetLoggedItems(unitOfWork);

        await WorkerShiftHelper.AddLoggedItemsToContext(unitOfWork, currentUserProvider, loggedItems,
            ShiftMasterActivityType.EndWorkerShift, firstActivity.WorkerCode, firstActivity.CenterCode,
            firstActivity.Id, cancellationToken);

        await unitOfWork.CompleteAsync(cancellationToken);

        return Result.Created;
    }
}
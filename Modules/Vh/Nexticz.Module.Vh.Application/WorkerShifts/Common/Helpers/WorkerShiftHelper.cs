using Nexticz.Module.Vh.Domain.ShiftMasterChanges;
using Nexticz.Module.Vh.Domain.WorkerShifts;
using Nexticz.Lib.Shared.Helpers;
using Nexticz.Lib.Shared.UserProviders;
using Interfaces_IUnitOfWork = Nexticz.Module.Vh.Application.Common.Interfaces.IUnitOfWork;
using IUnitOfWork = Nexticz.Module.Vh.Application.Common.Interfaces.IUnitOfWork;

namespace Nexticz.Module.Vh.Application.WorkerShifts.Common.Helpers;

public static class WorkerShiftHelper
{
    public static async Task AddLoggedItemsToContext(Interfaces_IUnitOfWork unitOfWork, ICurrentUserProvider currentUserProvider,
        List<LoggedItem> loggedItems, ShiftMasterActivityType shiftMasterActivityType, string workerCode,
        string centerCode, Guid workerShiftId,
        CancellationToken cancellationToken)
    {
        var shiftMasterChange = new ShiftMasterChange(currentUserProvider.GetCurrentUser().Email!,
            shiftMasterActivityType, workerCode, centerCode, workerShiftId);

        List<WorkerShiftChange> loggedWorkerShifts = [];
        List<WorkerShiftActivityChange> loggedWorkerShiftActivities = [];

        foreach (var loggedItem in loggedItems)
            switch (loggedItem.LoggedObject)
            {
                case WorkerShift workerShift:
                    loggedWorkerShifts.Add(new WorkerShiftChange(
                        loggedItem.Type,
                        workerShift.Id,
                        workerShift.Start,
                        workerShift.End,
                        workerShift.WorkerCode,
                        workerShift.WorkerCenterCode,
                        workerShift.Approved));
                    break;
                case WorkerShiftActivity workerShiftActivity:
                    loggedWorkerShiftActivities.Add(new WorkerShiftActivityChange(
                        loggedItem.Type,
                        workerShiftActivity.Id,
                        workerShiftActivity.WorkerShiftId,
                        workerShiftActivity.Start,
                        workerShiftActivity.End,
                        workerShiftActivity.WorkerCode,
                        workerShiftActivity.CenterCode,
                        workerShiftActivity.ActivityCode,
                        workerShiftActivity.ActivityCutOff,
                        workerShiftActivity.ActivityType,
                        workerShiftActivity.ActivitySource,
                        workerShiftActivity.LoadingDeviceId,
                        workerShiftActivity.Note,
                        workerShiftActivity.ActivitiesCount,
                        workerShiftActivity.Unit,
                        workerShiftActivity.Coefficient,
                        workerShiftActivity.Score,
                        workerShiftActivity.ActivityCountOrDurationMinutes,
                        workerShiftActivity.DurationMinutes,
                        workerShiftActivity.LastAddedActivityStart));
                    break;
            }

        foreach (var loggedWorkerShift in loggedWorkerShifts)
        {
            if (loggedWorkerShift.ContextChangeType != ContextChangeType.ChangedTo)
            {
                shiftMasterChange.WorkerShiftChanges.Add(loggedWorkerShift);
                continue;
            }

            shiftMasterChange.WorkerShiftChanges.Add(loggedWorkerShift);

            var itemBeforeChange = await unitOfWork.WorkerShiftsRepository.GetWorkerShiftByIdAsNoTrackingAsync(
                loggedWorkerShift.WorkerShiftId, cancellationToken);

            shiftMasterChange.WorkerShiftChanges.Add(new WorkerShiftChange(
                ContextChangeType.ChangedFrom,
                itemBeforeChange!.Id,
                itemBeforeChange.Start,
                itemBeforeChange.End,
                itemBeforeChange.WorkerCode,
                itemBeforeChange.WorkerCenterCode,
                itemBeforeChange.Approved));
        }

        foreach (var loggedWorkerShiftActivity in loggedWorkerShiftActivities)
        {
            if (loggedWorkerShiftActivity.ContextChangeType != ContextChangeType.ChangedTo)
            {
                shiftMasterChange.WorkerShiftActivityChanges.Add(loggedWorkerShiftActivity);
                continue;
            }

            shiftMasterChange.WorkerShiftActivityChanges.Add(loggedWorkerShiftActivity);

            var itemBeforeChange =
                await unitOfWork.WorkerShiftsRepository.GetWorkerShiftActivityByIdAsNoTrackingAsync(
                    loggedWorkerShiftActivity.WorkerShiftActivityId, cancellationToken);
            shiftMasterChange.WorkerShiftActivityChanges.Add(new WorkerShiftActivityChange(
                ContextChangeType.ChangedFrom,
                itemBeforeChange!.Id,
                itemBeforeChange.WorkerShiftId,
                itemBeforeChange.Start,
                itemBeforeChange.End,
                itemBeforeChange.WorkerCode,
                itemBeforeChange.CenterCode,
                itemBeforeChange.ActivityCode,
                itemBeforeChange.ActivityCutOff,
                itemBeforeChange.ActivityType,
                itemBeforeChange.ActivitySource,
                itemBeforeChange.LoadingDeviceId,
                itemBeforeChange.Note,
                itemBeforeChange.ActivitiesCount,
                itemBeforeChange.Unit,
                itemBeforeChange.Coefficient,
                itemBeforeChange.Score,
                itemBeforeChange.ActivityCountOrDurationMinutes,
                itemBeforeChange.DurationMinutes,
                itemBeforeChange.LastAddedActivityStart));
        }

        unitOfWork.Add(shiftMasterChange);
    }
}
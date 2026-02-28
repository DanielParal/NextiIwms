using ErrorOr;
using MediatR;
using Nexticz.Module.Vh.Domain.ShiftMasterChanges;
using Nexticz.Module.Vh.Domain.WorkerShifts;
using Nexticz.Lib.Shared.Helpers;
using Nexticz.Lib.Shared.UserProviders;
using Nexticz.Module.Vh.Application.WorkerShifts.Common.Helpers;
using Interfaces_IUnitOfWork = Nexticz.Module.Vh.Application.Common.Interfaces.IUnitOfWork;
using IUnitOfWork = Nexticz.Module.Vh.Application.Common.Interfaces.IUnitOfWork;

namespace Nexticz.Module.Vh.Application.WorkerShifts.Commands.AddWorkerShiftActivity;

public class AddWorkerShiftActivityCommandHandler(Interfaces_IUnitOfWork unitOfWork, ICurrentUserProvider currentUserProvider)
    : IRequestHandler<AddWorkerShiftActivityCommand, ErrorOr<Created>>
{
    public async Task<ErrorOr<Created>> Handle(AddWorkerShiftActivityCommand command,
        CancellationToken cancellationToken)
    {
        var workerShift =
            await unitOfWork.WorkerShiftsRepository.GetWorkerShiftByIdAsync(
                command.Id, cancellationToken);

        if (workerShift is null)
            return WorkerShiftErrors.WorkerShiftWithIdDoesNotExist;

        var activity = new WorkerShiftActivity(
            command.AddWorkerShiftActivityRequest.Start,
            command.AddWorkerShiftActivityRequest.WorkerCode,
            command.AddWorkerShiftActivityRequest.CenterCode,
            command.AddWorkerShiftActivityRequest.ActivityCode,
            command.AddWorkerShiftActivityRequest.DepositorCode,
            command.AddWorkerShiftActivityRequest.DepositorGroupCode,
            command.AddWorkerShiftActivityRequest.ActivitySystemType,
            Enum.Parse<ActivityType>(command.AddWorkerShiftActivityRequest.ActivityType.ToString()),
            Enum.Parse<ActivitySource>(command.AddWorkerShiftActivityRequest.ActivitySource.ToString()))
        {
            Note = command.AddWorkerShiftActivityRequest.Note,
            ActivityCutOff = command.AddWorkerShiftActivityRequest.ActivityCutOff,
            Coefficient = command.AddWorkerShiftActivityRequest.Coefficient,
            Unit = command.AddWorkerShiftActivityRequest.Unit,
            ActivityName = command.AddWorkerShiftActivityRequest.ActivityName
        };

        workerShift.AddActivity(activity);

        var loggedItems = LogContextChanges.GetLoggedItems(unitOfWork);

        await WorkerShiftHelper.AddLoggedItemsToContext(unitOfWork, currentUserProvider, loggedItems,
            ShiftMasterActivityType.AddActivity, activity.WorkerCode, activity.CenterCode, activity.Id,
            cancellationToken);

        await unitOfWork.CompleteAsync(cancellationToken);

        return Result.Created;
    }
}
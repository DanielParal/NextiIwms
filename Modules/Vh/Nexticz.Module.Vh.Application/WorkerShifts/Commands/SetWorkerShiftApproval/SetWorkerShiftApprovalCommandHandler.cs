using ErrorOr;
using MediatR;
using Nexticz.Module.Vh.Domain.ShiftMasterChanges;
using Nexticz.Module.Vh.Domain.WorkerShifts;
using Nexticz.Lib.Shared.Helpers;
using Nexticz.Lib.Shared.UserProviders;
using Nexticz.Module.Vh.Application.WorkerShifts.Common.Helpers;
using Nexticz.Module.Vh.Application.WorkerShifts.Notifications.WorkerShiftApprovalChanged;
using Interfaces_IUnitOfWork = Nexticz.Module.Vh.Application.Common.Interfaces.IUnitOfWork;
using IUnitOfWork = Nexticz.Module.Vh.Application.Common.Interfaces.IUnitOfWork;

namespace Nexticz.Module.Vh.Application.WorkerShifts.Commands.SetWorkerShiftApproval;

public class SetWorkerShiftApprovalCommandHandler(
    Interfaces_IUnitOfWork unitOfWork,
    ICurrentUserProvider currentUserProvider,
    IMediator mediator)
    : IRequestHandler<SetWorkerShiftApprovalCommand, ErrorOr<Created>>
{
    public async Task<ErrorOr<Created>> Handle(SetWorkerShiftApprovalCommand command,
        CancellationToken cancellationToken)
    {
        var workerShift =
            await unitOfWork.WorkerShiftsRepository.GetWorkerShiftByIdAsync(command.Id, cancellationToken);

        if (workerShift is null)
            return WorkerShiftErrors.WorkerShiftWithIdDoesNotExist;

        workerShift.SetApproval(command.SetWorkerShiftApprovalRequest.Approval);

        var loggedItems = LogContextChanges.GetLoggedItems(unitOfWork);

        var activityType = command.SetWorkerShiftApprovalRequest.Approval
            ? ShiftMasterActivityType.ApproveWorkerShift
            : ShiftMasterActivityType.DisApproveWorkerShift;

        var firstActivity = workerShift.Activities.First();

        await WorkerShiftHelper.AddLoggedItemsToContext(unitOfWork, currentUserProvider, loggedItems,
            activityType, firstActivity.WorkerCode, firstActivity.CenterCode, firstActivity.Id, cancellationToken);

        await unitOfWork.CompleteAsync(cancellationToken);

        var notification =
            new WorkerShiftApprovalChangedNotification(command.Id, command.SetWorkerShiftApprovalRequest.Approval);

        await mediator.Publish(notification, cancellationToken);

        return Result.Created;
    }
}
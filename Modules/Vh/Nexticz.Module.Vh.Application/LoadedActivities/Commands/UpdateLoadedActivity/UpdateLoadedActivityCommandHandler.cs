using ErrorOr;
using MediatR;
using Nexticz.Module.Vh.Domain.LoadedActivities;
using Nexticz.Module.Vh.Domain.WorkerShifts;
using Nexticz.Module.Vh.Application.Common.Interfaces;

namespace Nexticz.Module.Vh.Application.LoadedActivities.Commands.UpdateLoadedActivity;

public class UpdateLoadedActivityCommandHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<UpdateLoadedActivityCommand, ErrorOr<Updated>>
{
    public async Task<ErrorOr<Updated>> Handle(UpdateLoadedActivityCommand command, CancellationToken cancellationToken)
    {
        var loadedActivity =
            await unitOfWork.LoadedActivitiesRepository.GetLoadedActivityByIdAsync(command.Id, cancellationToken);

        if (loadedActivity is null) return LoadedActivitiesErrors.LoadedActivityWithIdDoesnotExist;

        loadedActivity.WorkerCode = command.UpdateLoadedActivityRequest.WorkerCode;
        loadedActivity.ActivityState =
            Enum.Parse<ActivityState>(command.UpdateLoadedActivityRequest.ActivityState.ToString());

        await unitOfWork.CompleteAsync(cancellationToken);

        return Result.Updated;
    }
}
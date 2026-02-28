using ErrorOr;
using MediatR;
using Nexticz.Module.Vh.Domain.NonDispensingActivities;
using Nexticz.Module.Vh.Application.Common.Interfaces;

namespace Nexticz.Module.Vh.Application.NonDispensingActivities.Commands.UpdateNonDispensingActivity;

public class UpdateNonDispensingActivityCommandHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<UpdateNonDispensingActivityCommand, ErrorOr<Updated>>
{
    public async Task<ErrorOr<Updated>> Handle(UpdateNonDispensingActivityCommand command,
        CancellationToken cancellationToken)
    {
        var nonDispensingActivity =
            await unitOfWork.NonDispensingActivitiesRepository.GetNonDispensingActivityByIdAsync(command.Id,
                cancellationToken);

        if (nonDispensingActivity is null) return NonDispensingActivityErrors.NonDispensingActivityWithIdDoesnotExist;

        nonDispensingActivity.Name = command.UpdateNonDispensingActivityRequest.Name;
        nonDispensingActivity.RequireNote = command.UpdateNonDispensingActivityRequest.RequireNote;
        nonDispensingActivity.Note = command.UpdateNonDispensingActivityRequest.Note;
        nonDispensingActivity.Unit = command.UpdateNonDispensingActivityRequest.Unit;
        nonDispensingActivity.Coefficient = command.UpdateNonDispensingActivityRequest.Coefficient;
        nonDispensingActivity.CutOff = command.UpdateNonDispensingActivityRequest.CutOff;
        nonDispensingActivity.CenterId = command.UpdateNonDispensingActivityRequest.CenterId;
        nonDispensingActivity.ActivityCategoryId = command.UpdateNonDispensingActivityRequest.ActivityCategoryId;

        await unitOfWork.CompleteAsync(cancellationToken);

        return Result.Updated;
    }
}
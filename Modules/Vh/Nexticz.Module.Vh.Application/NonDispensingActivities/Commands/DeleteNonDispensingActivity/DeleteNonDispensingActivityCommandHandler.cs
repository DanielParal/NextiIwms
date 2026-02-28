using ErrorOr;
using MediatR;
using Nexticz.Module.Vh.Domain.NonDispensingActivities;
using Nexticz.Module.Vh.Application.Common.Interfaces;

namespace Nexticz.Module.Vh.Application.NonDispensingActivities.Commands.DeleteNonDispensingActivity;

public class DeleteNonDispensingActivityCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<DeleteNonDispensingActivityCommand, ErrorOr<Deleted>>
{
    public async Task<ErrorOr<Deleted>> Handle(DeleteNonDispensingActivityCommand command, CancellationToken cancellationToken)
    {
        var nonDispensingActivity =
            await unitOfWork.NonDispensingActivitiesRepository.GetNonDispensingActivityByIdAsync(command.Id,
                cancellationToken);
        if (nonDispensingActivity is null) return NonDispensingActivityErrors.NonDispensingActivityWithIdDoesnotExist;
        
        unitOfWork.Remove(nonDispensingActivity);

        await unitOfWork.CompleteAsync(cancellationToken);

        return Result.Deleted;
    }
}
using ErrorOr;
using MediatR;
using Nexticz.Module.Vh.Domain.SystemActivities;
using Nexticz.Module.Vh.Application.Common.Interfaces;

namespace Nexticz.Module.Vh.Application.SystemActivities.Commands.DeleteSystemActivity;

public class DeleteSystemActivityCommandHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<DeleteSystemActivityCommand, ErrorOr<Deleted>>
{
    public async Task<ErrorOr<Deleted>> Handle(DeleteSystemActivityCommand command, CancellationToken cancellationToken)
    {
        var systemActivity =
            await unitOfWork.SystemActivitiesRepository.GetSystemActivityByIdAsync(command.Id, cancellationToken);

        if (systemActivity is null) return SystemActivityErrors.SystemActivityWithIdDoesnotExist;

        unitOfWork.Remove(systemActivity);

        await unitOfWork.CompleteAsync(cancellationToken);

        return Result.Deleted;
    }
}
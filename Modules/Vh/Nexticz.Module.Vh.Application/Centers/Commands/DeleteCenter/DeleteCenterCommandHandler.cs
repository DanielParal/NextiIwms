using ErrorOr;
using MediatR;
using Nexticz.Module.Vh.Domain.Centers;
using Nexticz.Module.Vh.Application.Common.Interfaces;

namespace Nexticz.Module.Vh.Application.Centers.Commands.DeleteCenter;

public class DeleteCenterCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<DeleteCenterCommand, ErrorOr<Deleted>>
{
    public async Task<ErrorOr<Deleted>> Handle(DeleteCenterCommand command, CancellationToken cancellationToken)
    {
        var center = await unitOfWork.CentersRepository.GetCenterByIdAsync(command.Id, cancellationToken);

        if (center is null) return CenterErrors.CenterWithIdDoesnotExist;

        unitOfWork.Remove(center);

        await unitOfWork.CompleteAsync(cancellationToken);

        return Result.Deleted;
    }
}
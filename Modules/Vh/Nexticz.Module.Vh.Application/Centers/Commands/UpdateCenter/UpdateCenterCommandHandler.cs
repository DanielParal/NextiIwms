using ErrorOr;
using MediatR;
using Nexticz.Module.Vh.Domain.Centers;
using Nexticz.Module.Vh.Application.Common.Interfaces;

namespace Nexticz.Module.Vh.Application.Centers.Commands.UpdateCenter;

public class UpdateCenterCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<UpdateCenterCommand, ErrorOr<Updated>>
{
    public async Task<ErrorOr<Updated>> Handle(UpdateCenterCommand command, CancellationToken cancellationToken)
    {
        var center = await unitOfWork.CentersRepository.GetCenterByIdAsync(command.Id, cancellationToken);

        if (center is null) return CenterErrors.CenterWithIdDoesnotExist;

        center.Code = command.UpdateCenterRequest.Code;
        center.Name = command.UpdateCenterRequest.Name;
        center.ShowDashboardSalaryData = command.UpdateCenterRequest.ShowDashboardSalaryData;

        await unitOfWork.CompleteAsync(cancellationToken);

        return Result.Updated;
    }
}
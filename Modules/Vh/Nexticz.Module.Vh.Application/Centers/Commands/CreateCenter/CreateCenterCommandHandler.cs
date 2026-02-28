using ErrorOr;
using MediatR;
using Nexticz.Module.Vh.Domain.Centers;
using Nexticz.Module.Vh.Application.Common.Interfaces;

namespace Nexticz.Module.Vh.Application.Centers.Commands.CreateCenter;

public class CreateCenterCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<CreateCenterCommand, ErrorOr<Created>>
{
    public async Task<ErrorOr<Created>> Handle(CreateCenterCommand command, CancellationToken cancellationToken)
    {
        var createdCenter = new Center
        {
            Name = command.CreateCenterRequest.Name, Code = command.CreateCenterRequest.Code,
            ShowDashboardSalaryData = command.CreateCenterRequest.ShowDashboardSalaryData
        };

        unitOfWork.Add(createdCenter);

        await unitOfWork.CompleteAsync(cancellationToken);

        return Result.Created;
    }
}
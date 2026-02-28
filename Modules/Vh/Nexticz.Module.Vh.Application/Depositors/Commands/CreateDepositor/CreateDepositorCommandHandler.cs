using ErrorOr;
using MediatR;
using Nexticz.Module.Vh.Domain.Depositors;
using Nexticz.Module.Vh.Application.Common.Interfaces;

namespace Nexticz.Module.Vh.Application.Depositors.Commands.CreateDepositor;

public class CreateDepositorCommandHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<CreateDepositorCommand, ErrorOr<Created>>
{
    public async Task<ErrorOr<Created>> Handle(CreateDepositorCommand command, CancellationToken cancellationToken)
    {
        var createdDepositor = new Depositor
        {
            Name = command.CreateDepositorRequest.Name,
            Code = command.CreateDepositorRequest.Code,
            CenterId = command.CreateDepositorRequest.CenterId,
            DepositorsGroupId = command.CreateDepositorRequest.DepositorsGroupId
        };

        unitOfWork.Add(createdDepositor);

        await unitOfWork.CompleteAsync(cancellationToken);

        return Result.Created;
    }
}
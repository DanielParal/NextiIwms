using ErrorOr;
using MediatR;
using Nexticz.Module.Vh.Domain.Depositors;
using Nexticz.Module.Vh.Application.Common.Interfaces;

namespace Nexticz.Module.Vh.Application.Depositors.Commands.UpdateDepositor;

public class UpdateDepositorCommandHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<UpdateDepositorCommand, ErrorOr<Updated>>
{
    public async Task<ErrorOr<Updated>> Handle(UpdateDepositorCommand command, CancellationToken cancellationToken)
    {
        var depositor = await unitOfWork.DepositorsRepository.GetDepositorByIdAsync(command.Id, cancellationToken);

        if (depositor is null) return DepositorErrors.DepositorWithIdDoesnotExist;

        depositor.Code = command.UpdateDepositorRequest.Code;
        depositor.Name = command.UpdateDepositorRequest.Name;
        depositor.DepositorsGroupId = command.UpdateDepositorRequest.DepositorsGroupId;
        depositor.CenterId = command.UpdateDepositorRequest.CenterId;

        await unitOfWork.CompleteAsync(cancellationToken);

        return Result.Updated;
    }
}
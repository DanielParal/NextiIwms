using ErrorOr;
using MediatR;
using Nexticz.Module.Vh.Domain.Depositors;
using Nexticz.Module.Vh.Application.Common.Interfaces;

namespace Nexticz.Module.Vh.Application.Depositors.Commands.DeleteDepositor;

public class DeleteDepositorCommandHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<DeleteDepositorCommand, ErrorOr<Deleted>>
{
    public async Task<ErrorOr<Deleted>> Handle(DeleteDepositorCommand command, CancellationToken cancellationToken)
    {
        var depositor = await unitOfWork.DepositorsRepository.GetDepositorByIdAsync(command.Id, cancellationToken);

        if (depositor is null) return DepositorErrors.DepositorWithIdDoesnotExist;

        unitOfWork.Remove(depositor);

        await unitOfWork.CompleteAsync(cancellationToken);

        return Result.Deleted;
    }
}
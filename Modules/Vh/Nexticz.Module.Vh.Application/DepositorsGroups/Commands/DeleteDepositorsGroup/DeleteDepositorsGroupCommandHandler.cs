using ErrorOr;
using MediatR;
using Nexticz.Module.Vh.Domain.DepositorsGroups;
using Nexticz.Module.Vh.Application.Common.Interfaces;

namespace Nexticz.Module.Vh.Application.DepositorsGroups.Commands.DeleteDepositorsGroup;

public class DeleteDepositorsGroupCommandHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<DeleteDepositorsGroupCommand, ErrorOr<Deleted>>
{
    public async Task<ErrorOr<Deleted>> Handle(DeleteDepositorsGroupCommand command,
        CancellationToken cancellationToken)
    {
        var depositorsGroup =
            await unitOfWork.DepositorsGroupsRepository.GetDepositorsGroupByIdAsync(command.Id, cancellationToken);

        if (depositorsGroup is null) return DepositorsGroupErrors.DepositorsGroupWithIdDoesnotExist;

        unitOfWork.Remove(depositorsGroup);

        await unitOfWork.CompleteAsync(cancellationToken);

        return Result.Deleted;
    }
}
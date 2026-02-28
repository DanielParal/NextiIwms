using ErrorOr;
using MediatR;
using Nexticz.Module.Vh.Domain.DepositorsGroups;
using Nexticz.Module.Vh.Application.Common.Interfaces;

namespace Nexticz.Module.Vh.Application.DepositorsGroups.Commands.UpdateDepositorsGroup;

public class UpdateDepositorsGroupCommandHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<UpdateDepositorsGroupCommand, ErrorOr<Updated>>
{
    public async Task<ErrorOr<Updated>> Handle(UpdateDepositorsGroupCommand command,
        CancellationToken cancellationToken)
    {
        var depositorsGroup =
            await unitOfWork.DepositorsGroupsRepository.GetDepositorsGroupByIdAsync(command.Id, cancellationToken);

        if (depositorsGroup is null) return DepositorsGroupErrors.DepositorsGroupWithIdDoesnotExist;

        depositorsGroup.Code = command.UpdateDepositorsGroupRequest.Code;
        depositorsGroup.Name = command.UpdateDepositorsGroupRequest.Name;
        depositorsGroup.CenterId = command.UpdateDepositorsGroupRequest.CenterId;

        await unitOfWork.CompleteAsync(cancellationToken);

        return Result.Updated;
    }
}
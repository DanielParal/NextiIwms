using ErrorOr;
using MediatR;
using Nexticz.Module.Vh.Domain.DepositorsGroups;
using Nexticz.Module.Vh.Application.Common.Interfaces;

namespace Nexticz.Module.Vh.Application.DepositorsGroups.Commands.CreateDepositorsGroup;

public class CreateDepositorsGroupCommandHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<CreateDepositorsGroupCommand, ErrorOr<Created>>
{
    public async Task<ErrorOr<Created>> Handle(CreateDepositorsGroupCommand command,
        CancellationToken cancellationToken)
    {
        var depositorsGroup = new DepositorsGroup
        {
            Name = command.CreateDepositorsGroupRequest.Name,
            Code = command.CreateDepositorsGroupRequest.Code,
            CenterId = command.CreateDepositorsGroupRequest.CenterId
        };

        unitOfWork.Add(depositorsGroup);

        await unitOfWork.CompleteAsync(cancellationToken);

        return Result.Created;
    }
}
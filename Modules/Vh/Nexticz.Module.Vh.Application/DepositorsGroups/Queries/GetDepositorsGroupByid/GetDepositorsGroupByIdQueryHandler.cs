using ErrorOr;
using MediatR;
using Nexticz.Module.Vh.Contracts.DepositorsGroups;
using Nexticz.Module.Vh.Domain.DepositorsGroups;
using Nexticz.Module.Vh.Application.Common.Interfaces;

namespace Nexticz.Module.Vh.Application.DepositorsGroups.Queries.GetDepositorsGroupByid;

public class
    GetDepositorsGroupByIdQueryHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<GetDepositorsGroupByIdQuery, ErrorOr<DepositorsGroupResponse>>
{
    public async Task<ErrorOr<DepositorsGroupResponse>> Handle(GetDepositorsGroupByIdQuery query,
        CancellationToken cancellationToken)
    {
        var depositorsGroup =
            await unitOfWork.DepositorsGroupsRepository.GetDepositorGroupResponseByIdAsync(query.Id, cancellationToken);

        if (depositorsGroup is null) return DepositorsGroupErrors.DepositorsGroupWithIdDoesnotExist;

        return depositorsGroup;
    }
}
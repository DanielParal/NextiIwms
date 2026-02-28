using ErrorOr;
using MediatR;
using Nexticz.Module.Vh.Contracts.DepositorsGroups;

namespace Nexticz.Module.Vh.Application.DepositorsGroups.Queries.GetDepositorsGroupByid;

public class GetDepositorsGroupByIdQuery : IRequest<ErrorOr<DepositorsGroupResponse>>
{
    public required Guid Id { get; set; }
}
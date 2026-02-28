using ErrorOr;
using MediatR;
using Nexticz.Module.Vh.Contracts.DepositorsGroups;

namespace Nexticz.Module.Vh.Application.DepositorsGroups.Commands.CreateDepositorsGroup;

public class CreateDepositorsGroupCommand : IRequest<ErrorOr<Created>>
{
    public required CreateDepositorsGroupRequest CreateDepositorsGroupRequest { get; set; }
}
using ErrorOr;
using MediatR;
using Nexticz.Module.Vh.Contracts.DepositorsGroups;

namespace Nexticz.Module.Vh.Application.DepositorsGroups.Commands.UpdateDepositorsGroup;

public class UpdateDepositorsGroupCommand : IRequest<ErrorOr<Updated>>
{
    public required Guid Id { get; set; }
    public required UpdateDepositorsGroupRequest UpdateDepositorsGroupRequest { get; set; }
}
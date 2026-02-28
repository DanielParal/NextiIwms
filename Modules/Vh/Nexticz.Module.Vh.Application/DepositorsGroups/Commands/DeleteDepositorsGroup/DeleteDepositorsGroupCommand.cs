using ErrorOr;
using MediatR;

namespace Nexticz.Module.Vh.Application.DepositorsGroups.Commands.DeleteDepositorsGroup;

public class DeleteDepositorsGroupCommand : IRequest<ErrorOr<Deleted>>
{
    public required Guid Id { get; set; }
}
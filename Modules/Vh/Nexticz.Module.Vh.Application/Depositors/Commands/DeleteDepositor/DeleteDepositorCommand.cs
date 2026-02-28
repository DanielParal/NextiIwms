using ErrorOr;
using MediatR;

namespace Nexticz.Module.Vh.Application.Depositors.Commands.DeleteDepositor;

public class DeleteDepositorCommand : IRequest<ErrorOr<Deleted>>
{
    public required Guid Id { get; set; }
}
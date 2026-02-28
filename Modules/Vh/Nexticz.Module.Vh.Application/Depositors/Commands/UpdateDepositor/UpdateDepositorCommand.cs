using ErrorOr;
using MediatR;
using Nexticz.Module.Vh.Contracts.Depositors;

namespace Nexticz.Module.Vh.Application.Depositors.Commands.UpdateDepositor;

public class UpdateDepositorCommand : IRequest<ErrorOr<Updated>>
{
    public required Guid Id { get; set; }
    public required UpdateDepositorRequest UpdateDepositorRequest { get; set; }
}
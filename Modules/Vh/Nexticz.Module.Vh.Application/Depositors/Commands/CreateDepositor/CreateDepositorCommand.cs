using ErrorOr;
using MediatR;
using Nexticz.Module.Vh.Contracts.Depositors;

namespace Nexticz.Module.Vh.Application.Depositors.Commands.CreateDepositor;

public class CreateDepositorCommand : IRequest<ErrorOr<Created>>
{
    public required CreateDepositorRequest CreateDepositorRequest { get; set; }
}
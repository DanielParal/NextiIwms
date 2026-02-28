using ErrorOr;
using MediatR;
using Nexticz.Module.Vh.Contracts.Centers;

namespace Nexticz.Module.Vh.Application.Centers.Commands.CreateCenter;

public class CreateCenterCommand : IRequest<ErrorOr<Created>>
{
    public required CreateCenterRequest CreateCenterRequest { get; set; }
}
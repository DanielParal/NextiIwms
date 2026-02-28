using ErrorOr;
using MediatR;

namespace Nexticz.Module.Vh.Application.Centers.Commands.DeleteCenter;

public class DeleteCenterCommand : IRequest<ErrorOr<Deleted>>
{
    public required Guid Id { get; set; }
}
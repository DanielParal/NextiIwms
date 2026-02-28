using ErrorOr;
using MediatR;

namespace Nexticz.Module.Vh.Application.SystemActivities.Commands.DeleteSystemActivity;

public class DeleteSystemActivityCommand : IRequest<ErrorOr<Deleted>>
{
    public required Guid Id { get; set; }
}
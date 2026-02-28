using ErrorOr;
using MediatR;

namespace Nexticz.Module.Vh.Application.NonDispensingActivities.Commands.DeleteNonDispensingActivity;

public class DeleteNonDispensingActivityCommand : IRequest<ErrorOr<Deleted>>
{
    public required Guid Id { get; set; }
}
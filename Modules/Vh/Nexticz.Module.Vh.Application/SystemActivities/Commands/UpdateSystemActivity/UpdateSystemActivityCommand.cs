using ErrorOr;
using MediatR;
using Nexticz.Module.Vh.Contracts.SystemActivities;

namespace Nexticz.Module.Vh.Application.SystemActivities.Commands.UpdateSystemActivity;

public class UpdateSystemActivityCommand : IRequest<ErrorOr<Updated>>
{
    public required Guid Id { get; set; }
    public required UpdateSystemActivityRequest UpdateSystemActivityRequest { get; set; }
}
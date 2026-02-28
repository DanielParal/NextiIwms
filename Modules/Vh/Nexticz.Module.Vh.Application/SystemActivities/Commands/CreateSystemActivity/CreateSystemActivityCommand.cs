using ErrorOr;
using MediatR;
using Nexticz.Module.Vh.Contracts.SystemActivities;

namespace Nexticz.Module.Vh.Application.SystemActivities.Commands.CreateSystemActivity;

public class CreateSystemActivityCommand : IRequest<ErrorOr<Created>>
{
    public required CreateSystemActivityRequest CreateSystemActivityRequest { get; set; }
}
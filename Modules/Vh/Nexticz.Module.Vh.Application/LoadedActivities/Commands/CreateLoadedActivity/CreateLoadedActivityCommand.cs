using ErrorOr;
using MediatR;
using Nexticz.Module.Vh.Domain.LoadedActivities;

namespace Nexticz.Module.Vh.Application.LoadedActivities.Commands.CreateLoadedActivity;

public class CreateLoadedActivityCommand : IRequest<ErrorOr<Created>>
{
    public required LoadedActivity LoadedActivity { get; set; }
}
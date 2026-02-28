using ErrorOr;
using MediatR;
using Nexticz.Module.Vh.Contracts.LoadedActivities;

namespace Nexticz.Module.Vh.Application.LoadedActivities.Commands.UpdateLoadedActivity;

public record UpdateLoadedActivityCommand(Guid Id, UpdateLoadedActivityRequest UpdateLoadedActivityRequest)
    : IRequest<ErrorOr<Updated>>;
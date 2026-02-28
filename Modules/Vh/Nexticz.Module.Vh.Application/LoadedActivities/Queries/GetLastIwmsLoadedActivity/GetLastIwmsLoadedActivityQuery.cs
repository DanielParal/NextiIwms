using ErrorOr;
using MediatR;
using Nexticz.Module.Vh.Domain.LoadedActivities;

namespace Nexticz.Module.Vh.Application.LoadedActivities.Queries.GetLastIwmsLoadedActivity;

public class GetLastIwmsLoadedActivityQuery : IRequest<ErrorOr<LoadedActivity?>>
{
}
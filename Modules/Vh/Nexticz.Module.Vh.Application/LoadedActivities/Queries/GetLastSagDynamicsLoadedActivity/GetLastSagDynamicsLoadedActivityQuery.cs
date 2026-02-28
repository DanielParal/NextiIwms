using ErrorOr;
using MediatR;
using Nexticz.Module.Vh.Domain.LoadedActivities;

namespace Nexticz.Module.Vh.Application.LoadedActivities.Queries.GetLastSagDynamicsLoadedActivity;

public class GetLastSagDynamicsLoadedActivityQuery : IRequest<ErrorOr<LoadedActivity?>>
{
    
}
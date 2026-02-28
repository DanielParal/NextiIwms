using ErrorOr;
using MediatR;
using Nexticz.Module.Vh.Domain.LoadedActivities;
using Nexticz.Module.Vh.Application.Common.Interfaces;
using Nexticz.Module.Vh.Application.LoadedActivities.Queries.GetLastMyStockLoadedActivity;

namespace Nexticz.Module.Vh.Application.LoadedActivities.Queries.GetLastSagDynamicsLoadedActivity;

public class GetLastSagDynamicsLoadedActivityQueryHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetLastMyStockLoadedActivityQuery, ErrorOr<LoadedActivity?>>
{
    public async Task<ErrorOr<LoadedActivity?>> Handle(GetLastMyStockLoadedActivityQuery query, CancellationToken cancellationToken)
    {
        return await unitOfWork.LoadedActivitiesRepository.GetLastSagDynamicsLoadedActivityAsync(cancellationToken);
    }
}
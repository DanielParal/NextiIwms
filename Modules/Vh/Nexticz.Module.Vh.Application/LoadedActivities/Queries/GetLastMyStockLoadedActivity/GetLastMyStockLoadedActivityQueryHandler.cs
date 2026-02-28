using ErrorOr;
using MediatR;
using Nexticz.Module.Vh.Domain.LoadedActivities;
using Nexticz.Module.Vh.Application.Common.Interfaces;

namespace Nexticz.Module.Vh.Application.LoadedActivities.Queries.GetLastMyStockLoadedActivity;

public class GetLastMyStockLoadedActivityQueryHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetLastMyStockLoadedActivityQuery, ErrorOr<LoadedActivity?>>
{
    public async Task<ErrorOr<LoadedActivity?>> Handle(GetLastMyStockLoadedActivityQuery query, CancellationToken cancellationToken)
    {
        return await unitOfWork.LoadedActivitiesRepository.GetLastMyStockLoadedActivityAsync(cancellationToken);
    }
}
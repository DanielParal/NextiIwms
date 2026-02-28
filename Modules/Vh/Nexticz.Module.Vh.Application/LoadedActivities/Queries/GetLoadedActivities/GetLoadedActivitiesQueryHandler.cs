using ErrorOr;
using MediatR;
using Nexticz.Lib.Shared.DevExtreme;
using Nexticz.Module.Vh.Application.Common.Interfaces;


namespace Nexticz.Module.Vh.Application.LoadedActivities.Queries.GetLoadedActivities;

public class GetLoadedActivitiesQueryHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetLoadedActivitiesQuery, ErrorOr<FilteredResult>>
{
    public async Task<ErrorOr<FilteredResult>> Handle(GetLoadedActivitiesQuery query, CancellationToken cancellationToken)
    {
        return await unitOfWork.LoadedActivitiesRepository.GetLoadedActivitiesAsync(query.FilteringParams,
            cancellationToken);
    }
}
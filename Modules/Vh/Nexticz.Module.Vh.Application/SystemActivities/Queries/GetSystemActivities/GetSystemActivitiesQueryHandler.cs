using ErrorOr;
using MediatR;
using Nexticz.Lib.Shared.DevExtreme;
using Nexticz.Module.Vh.Application.Common.Interfaces;


namespace Nexticz.Module.Vh.Application.SystemActivities.Queries.GetSystemActivities;

public class GetSystemActivitiesQueryHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<GetSystemActivitiesQuery, ErrorOr<FilteredResult>>
{
    public async Task<ErrorOr<FilteredResult>> Handle(GetSystemActivitiesQuery query,
        CancellationToken cancellationToken)
    {
        return await unitOfWork.SystemActivitiesRepository.GetSystemActivitiesAsync(query.FilteringParams,
            cancellationToken);
    }
}
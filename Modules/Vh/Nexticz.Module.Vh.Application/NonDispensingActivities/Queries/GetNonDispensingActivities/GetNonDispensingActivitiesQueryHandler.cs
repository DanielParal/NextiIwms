using ErrorOr;
using MediatR;
using Nexticz.Lib.Shared.DevExtreme;
using Nexticz.Module.Vh.Application.Common.Interfaces;


namespace Nexticz.Module.Vh.Application.NonDispensingActivities.Queries.GetNonDispensingActivities;

public class GetNonDispensingActivitiesQueryHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetNonDispensingActivitiesQuery, ErrorOr<FilteredResult>>
{
    public async Task<ErrorOr<FilteredResult>> Handle(GetNonDispensingActivitiesQuery query, CancellationToken cancellationToken)
    {
        return await unitOfWork.NonDispensingActivitiesRepository.GetNonDispensingActivitiesAsync(query.FilteringParams,
            cancellationToken);
    }
}
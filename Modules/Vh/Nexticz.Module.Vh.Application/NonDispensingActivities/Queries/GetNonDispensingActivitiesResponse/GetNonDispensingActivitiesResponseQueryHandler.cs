using ErrorOr;
using MediatR;
using Nexticz.Lib.Shared.DevExtreme;
using Nexticz.Module.Vh.Application.Common.Interfaces;


namespace Nexticz.Module.Vh.Application.NonDispensingActivities.Queries.GetNonDispensingActivitiesResponse;

public class GetNonDispensingActivitiesResponseQueryHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<GetNonDispensingActivitiesResponseQuery, ErrorOr<FilteredResult>>
{
    public async Task<ErrorOr<FilteredResult>> Handle(GetNonDispensingActivitiesResponseQuery query,
        CancellationToken cancellationToken)
    {
        return await unitOfWork.NonDispensingActivitiesRepository.GetNonDispensingActivitiesResponseAsync(
            query.FilteringParams,
            cancellationToken);
    }
}
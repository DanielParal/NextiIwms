using ErrorOr;
using MediatR;
using Nexticz.Module.Vh.Contracts.NonDispensingActivities;
using Nexticz.Module.Vh.Domain.NonDispensingActivities;
using Nexticz.Module.Vh.Application.Common.Interfaces;

namespace Nexticz.Module.Vh.Application.NonDispensingActivities.Queries.GetNonDispensingActivityBySlug;

public class GetNonDispensingActivityBySlugQueryHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetNonDispensingActivityBySlugQuery, ErrorOr<NonDispensingActivityResponse>>
{
    public async Task<ErrorOr<NonDispensingActivityResponse>> Handle(GetNonDispensingActivityBySlugQuery query, CancellationToken cancellationToken)
    {
        var nonDispensingActivity =
            await unitOfWork.NonDispensingActivitiesRepository.GetNonDispensingActivityResponseBySlugAsync(query.Slug,
                cancellationToken);

        if (nonDispensingActivity is null) return NonDispensingActivityErrors.NonDispensingActivityWithIdDoesnotExist;

        return nonDispensingActivity;
    }
}
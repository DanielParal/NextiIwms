using ErrorOr;
using MediatR;
using Nexticz.Module.Vh.Contracts.NonDispensingActivities;
using Nexticz.Module.Vh.Domain.NonDispensingActivities;
using Nexticz.Module.Vh.Application.Common.Interfaces;

namespace Nexticz.Module.Vh.Application.NonDispensingActivities.Queries.GetNonDispensingActivityById;

public class GetNonDispensingActivityByIdQueryHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetNonDispensingActivityByIdQuery, ErrorOr<NonDispensingActivityResponse>>
{
    public async Task<ErrorOr<NonDispensingActivityResponse>> Handle(GetNonDispensingActivityByIdQuery query, CancellationToken cancellationToken)
    {
        var nonDispensingActivity =
            await unitOfWork.NonDispensingActivitiesRepository.GetNonDispensingActivityResponseByIdAsync(query.Id,
                cancellationToken);

        if (nonDispensingActivity is null) return NonDispensingActivityErrors.NonDispensingActivityWithIdDoesnotExist;

        return nonDispensingActivity;
    }
}
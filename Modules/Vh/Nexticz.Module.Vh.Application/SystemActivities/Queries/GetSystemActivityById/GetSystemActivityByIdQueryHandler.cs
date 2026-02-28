using ErrorOr;
using MediatR;
using Nexticz.Module.Vh.Contracts.SystemActivities;
using Nexticz.Module.Vh.Domain.SystemActivities;
using Nexticz.Module.Vh.Application.Common.Interfaces;

namespace Nexticz.Module.Vh.Application.SystemActivities.Queries.GetSystemActivityById;

public class GetSystemActivityByIdQueryHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<GetSystemActivityByIdQuery, ErrorOr<SystemActivityResponse>>
{
    public async Task<ErrorOr<SystemActivityResponse>> Handle(GetSystemActivityByIdQuery query,
        CancellationToken cancellationToken)
    {
        var systemActivity =
            await unitOfWork.SystemActivitiesRepository.GetSystemActivityResponseByIdAsync(query.Id, cancellationToken);

        if (systemActivity is null) return SystemActivityErrors.SystemActivityWithIdDoesnotExist;

        return systemActivity;
    }
}
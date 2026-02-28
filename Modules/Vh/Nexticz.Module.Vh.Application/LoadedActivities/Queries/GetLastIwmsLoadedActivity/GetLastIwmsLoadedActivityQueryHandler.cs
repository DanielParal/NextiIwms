using ErrorOr;
using MediatR;
using Nexticz.Module.Vh.Domain.LoadedActivities;
using Nexticz.Module.Vh.Application.Common.Interfaces;

namespace Nexticz.Module.Vh.Application.LoadedActivities.Queries.GetLastIwmsLoadedActivity;

public class GetLastIwmsLoadedActivityQueryHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<GetLastIwmsLoadedActivityQuery, ErrorOr<LoadedActivity?>>
{
    public async Task<ErrorOr<LoadedActivity?>> Handle(GetLastIwmsLoadedActivityQuery query,
        CancellationToken cancellationToken)
    {
        return await unitOfWork.LoadedActivitiesRepository.GetLastIwmsLoadedActivityAsync(cancellationToken);
    }
}
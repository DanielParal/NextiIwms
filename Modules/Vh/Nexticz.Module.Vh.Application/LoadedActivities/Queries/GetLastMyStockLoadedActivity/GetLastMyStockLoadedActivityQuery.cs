using ErrorOr;
using MediatR;
using Nexticz.Module.Vh.Domain.LoadedActivities;

namespace Nexticz.Module.Vh.Application.LoadedActivities.Queries.GetLastMyStockLoadedActivity;

public class GetLastMyStockLoadedActivityQuery : IRequest<ErrorOr<LoadedActivity?>>
{
}
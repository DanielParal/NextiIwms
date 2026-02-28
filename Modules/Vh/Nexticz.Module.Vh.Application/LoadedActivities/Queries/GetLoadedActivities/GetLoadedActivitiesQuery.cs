using ErrorOr;
using MediatR;
using Nexticz.Lib.Shared.DevExtreme;
using Nexticz.Module.Vh.Application.LoadedActivities.Common.Models;


namespace Nexticz.Module.Vh.Application.LoadedActivities.Queries.GetLoadedActivities;

public class GetLoadedActivitiesQuery : IRequest<ErrorOr<FilteredResult>>
{
    public required LoadedActivitiesFilteringParams FilteringParams { get; set; }
}
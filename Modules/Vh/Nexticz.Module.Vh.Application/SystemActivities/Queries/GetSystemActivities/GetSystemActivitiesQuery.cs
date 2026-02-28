using ErrorOr;
using MediatR;
using Nexticz.Lib.Shared.DevExtreme;
using Nexticz.Module.Vh.Application.SystemActivities.Common.Models;


namespace Nexticz.Module.Vh.Application.SystemActivities.Queries.GetSystemActivities;

public class GetSystemActivitiesQuery : IRequest<ErrorOr<FilteredResult>>
{
    public required SystemActivitiesFilteringParams FilteringParams { get; set; }
}
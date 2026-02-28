using ErrorOr;
using MediatR;
using Nexticz.Lib.Shared.DevExtreme;
using Nexticz.Module.Vh.Application.NonDispensingActivities.Common.Models;


namespace Nexticz.Module.Vh.Application.NonDispensingActivities.Queries.GetNonDispensingActivitiesResponse;

public class GetNonDispensingActivitiesResponseQuery : IRequest<ErrorOr<FilteredResult>>
{
    public required NonDispensingActivitiesFilteringParams FilteringParams { get; set; }
}
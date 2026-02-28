using ErrorOr;
using MediatR;
using Nexticz.Module.Vh.Contracts.NonDispensingActivities;
using Nexticz.Lib.Shared.DevExtreme;
using Nexticz.Module.Vh.Application.NonDispensingActivities.Common.Models;


namespace Nexticz.Module.Vh.Application.NonDispensingActivities.Queries.GetNonDispensingActivities;

public class GetNonDispensingActivitiesQuery : IRequest<ErrorOr<FilteredResult>>
{
    public required NonDispensingActivitiesFilteringParams FilteringParams { get; set; }
}
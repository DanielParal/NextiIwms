using ErrorOr;
using MediatR;
using Nexticz.Lib.Shared.DevExtreme;
using Nexticz.Module.Vh.Application.ReportActivities.Common.Models;


namespace Nexticz.Module.Vh.Application.Reports.Queries.GetReportActivities;

public class GetReportActivitiesQuery : IRequest<ErrorOr<FilteredResult>>
{
    public ReportActivitiesFilteringParams? FilteringParams { get; set; }
}
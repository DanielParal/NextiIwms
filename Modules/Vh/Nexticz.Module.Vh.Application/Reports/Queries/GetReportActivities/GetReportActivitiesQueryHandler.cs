using ErrorOr;
using MediatR;
using Nexticz.Lib.Shared.DevExtreme;
using Nexticz.Module.Vh.Application.Common.Interfaces;


namespace Nexticz.Module.Vh.Application.Reports.Queries.GetReportActivities;

public class GetReportActivitiesQueryHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<GetReportActivitiesQuery, ErrorOr<FilteredResult>>
{
    public async Task<ErrorOr<FilteredResult>> Handle(GetReportActivitiesQuery query,
        CancellationToken cancellationToken)
    {
        return await unitOfWork.ReportActivitiesRepository.GetReportActivitiesAsync(query.FilteringParams,
            cancellationToken);
    }
}
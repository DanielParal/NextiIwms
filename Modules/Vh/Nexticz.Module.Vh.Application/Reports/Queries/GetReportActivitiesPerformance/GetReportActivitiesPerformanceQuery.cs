using ErrorOr;
using MediatR;
using Nexticz.Module.Vh.Contracts.Reports;

namespace Nexticz.Module.Vh.Application.Reports.Queries.GetReportActivitiesPerformance;

public record GetReportActivitiesPerformanceQuery(PerformanceReportsRequest PerformanceReportsRequest) : IRequest<ErrorOr<ReportActivitiesPerformanceResponse>>;

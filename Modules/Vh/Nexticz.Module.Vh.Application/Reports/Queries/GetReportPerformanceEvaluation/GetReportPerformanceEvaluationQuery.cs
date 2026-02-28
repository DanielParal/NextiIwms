using ErrorOr;
using MediatR;
using Nexticz.Module.Vh.Contracts.Reports;
using Nexticz.Module.Vh.Application.ReportActivities.Common.Models;

namespace Nexticz.Module.Vh.Application.Reports.Queries.GetReportPerformanceEvaluation;

public record GetReportPerformanceEvaluationQuery(ReportPerformanceEvaluationFilteringParams FilteringParams)
    : IRequest<ErrorOr<ReportPerformanceEvaluationResponse>>;
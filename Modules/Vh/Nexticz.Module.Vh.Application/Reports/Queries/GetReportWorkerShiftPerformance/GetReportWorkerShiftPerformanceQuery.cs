using ErrorOr;
using MediatR;
using Nexticz.Module.Vh.Contracts.Reports;

namespace Nexticz.Module.Vh.Application.Reports.Queries.GetReportWorkerShiftPerformance;

public record GetReportWorkerShiftPerformanceQuery(PerformanceReportsRequest PerformanceReportsRequest) : IRequest<ErrorOr<ReportWorkerShiftsPerformanceResponse>>;

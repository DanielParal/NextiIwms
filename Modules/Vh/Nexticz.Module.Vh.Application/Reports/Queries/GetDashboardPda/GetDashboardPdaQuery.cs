using ErrorOr;
using MediatR;
using Nexticz.Module.Vh.Application.ReportActivities.Common.Models;
using Nexticz.Module.Vh.Contracts.Reports;

namespace Nexticz.Module.Vh.Application.Reports.Queries.GetDashboardPda;

public record GetDashboardPdaQuery(DashboardPdaRequest DashboardPdaRequest) : IRequest<ErrorOr<DashboardPdaResponse>>;
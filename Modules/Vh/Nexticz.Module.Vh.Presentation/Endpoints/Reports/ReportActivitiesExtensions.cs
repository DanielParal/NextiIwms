using Microsoft.AspNetCore.Routing;

namespace Nexticz.Module.Vh.Presentation.Endpoints.Reports;

public static class ReportActivitiesExtensions
{
    public static IEndpointRouteBuilder MapReportActivitiesEndpoints(this IEndpointRouteBuilder builder)
    {
        return builder
            .MapDashboard()
            .MapDashboardPda()
            .MapGetReportActivities()
            .MapGetReportWorkerShiftPerformance()
            .MapGetReportActivitiesPerformance()
            .MapReportPerformanceEvaluation();
    }
}
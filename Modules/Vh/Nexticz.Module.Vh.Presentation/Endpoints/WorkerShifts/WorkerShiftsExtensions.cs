using Microsoft.AspNetCore.Routing;

namespace Nexticz.Module.Vh.Presentation.Endpoints.WorkerShifts;

public static class WorkerShiftsExtensions
{
    public static IEndpointRouteBuilder MapWorkerShiftsEndpoints(this IEndpointRouteBuilder builder)
    {
        return builder
            .MapGetWorkerShifts()
            .MapAddWorkerShift()
            .MapEndWorkerShift()
            .MapAddWorkershiftActivity()
            .MapSetWorkerShiftApproval();
    }
}
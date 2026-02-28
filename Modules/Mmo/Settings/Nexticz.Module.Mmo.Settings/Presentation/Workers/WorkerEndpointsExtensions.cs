using Microsoft.AspNetCore.Routing;

namespace Nexticz.Module.Mmo.Settings.Presentation.Workers;

internal static class WorkerEndpointsExtensions
{
    public static IEndpointRouteBuilder MapWorkersEndpoints(this IEndpointRouteBuilder builder)
    {
        return builder
            .MapCreateWorker()
            .MapDeleteWorker()
            .MapUpdateWorker()
            .MapGetWorkers();
    }
    
    public static IEndpointRouteBuilder MapCompletionPermissionWorkersEndpoints(this IEndpointRouteBuilder builder)
    {
        return builder
            .MapGetWorkerByPin();
    }
}
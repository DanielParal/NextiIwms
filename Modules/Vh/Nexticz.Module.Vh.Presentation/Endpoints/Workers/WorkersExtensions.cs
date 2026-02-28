using Microsoft.AspNetCore.Routing;

namespace Nexticz.Module.Vh.Presentation.Endpoints.Workers;

public static class WorkersExtensions
{
    public static IEndpointRouteBuilder MapWorkersEndpoints(this IEndpointRouteBuilder builder)
    {
        return builder
            .MapCreateWorker()
            .MapUpdateWorker()
            .MapDeleteWorker()
            .MapGetWorkerByIdOrSlug()
            .MapGetWorkers();
    }
}
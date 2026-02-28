using Microsoft.AspNetCore.Routing;

namespace Nexticz.Module.Vh.Presentation.Endpoints.LoadingActionsNdas;

public static class LoadingActionsNdasExtensions
{
    public static IEndpointRouteBuilder MapLoadingActionsNdasEndpoints(this IEndpointRouteBuilder builder)
    {
        return builder
            .MapCreateLoadingActionsNda()
            .MapUpdateLoadingActionsNda()
            .MapDeleteLoadingActionsNda()
            .MapGetLoadingActionsNdaById()
            .MapGetLoadingActionsNdas();
    }
}
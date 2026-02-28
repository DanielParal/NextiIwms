using Microsoft.AspNetCore.Routing;

namespace Nexticz.Module.Vh.Presentation.Endpoints.Assortments;

public static class AssortmentsExtensions
{
    public static IEndpointRouteBuilder MapAssortmentsEndpoints(this IEndpointRouteBuilder builder)
    {
        return builder
            .MapCreateAssortment()
            .MapUpdateAssortment()
            .MapDeleteAssortment()
            .MapGetAssortmentById()
            .MapGetAssortments();
    }
}
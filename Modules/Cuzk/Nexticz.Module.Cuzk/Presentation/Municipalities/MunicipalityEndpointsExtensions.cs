using Microsoft.AspNetCore.Routing;

namespace Nexticz.Module.Cuzk.Presentation.Municipalities;

internal static class MunicipalityEndpointsExtensions
{
    public static IEndpointRouteBuilder MapMunicipalitiesEndpoints(this IEndpointRouteBuilder builder)
    {
        return builder
            .MapCreateMunicipalityEndpoint()
            .MapUpdateMunicipalityEndpoint()
            .MapDeleteMunicipalityEndpoint()
            .MapGetMunicipalityByCodeEndpoint()
            .MapGetMunicipalitiesEndpoint();
    }
}
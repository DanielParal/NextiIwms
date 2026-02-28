using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Nexticz.Module.Cuzk.Contracts.OpenApiContracts;

namespace Nexticz.Module.Cuzk.Presentation.OpenApiContracts;

internal static class GetOpenApiContractsEndpoint
{
    public static IEndpointRouteBuilder MapGetOpenApiContractsEndpoint(this IEndpointRouteBuilder builder)
    {
        builder.MapGet(CuzkEndpoints.OpenApiContractEndpoints.GetOpenApiContracts,
                Results.NoContent)
            .Produces<OpenApiResponse>()
            .HasApiVersion(1.0)
            .WithName(CuzkEndpoints.GetOpenApiName(nameof(CuzkEndpoints.OpenApiContractEndpoints.GetOpenApiContracts)));

        return builder;
    }
}
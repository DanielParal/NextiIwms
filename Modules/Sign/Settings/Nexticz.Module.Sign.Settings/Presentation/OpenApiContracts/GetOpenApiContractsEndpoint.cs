using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Nexticz.Module.Sign.Settings.Contracts.OpenApiContracts;

namespace Nexticz.Module.Sign.Settings.Presentation.OpenApiContracts;

internal static class GetOpenApiContractsEndpoint
{
    public static IEndpointRouteBuilder MapGetOpenApiContractsEndpoint(this IEndpointRouteBuilder builder)
    {
        builder.MapGet(SettingsEndpoints.OpenApiContractEndpoints.GetOpenApiContracts,
                () => Results.NoContent())
            .Produces<OpenApiResponse>()
            .HasApiVersion(1.0)
            .WithName(SettingsEndpoints.GetOpenApiName(nameof(SettingsEndpoints.OpenApiContractEndpoints.GetOpenApiContracts)));

        return builder;
    }
}
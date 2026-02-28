using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Nexticz.Module.Notifications.Contracts.OpenApiContracts;

namespace Nexticz.Module.Notifications.Presentation.OpenApiContracts;

internal static class GetOpenApiContractsEndpoint
{
    public static IEndpointRouteBuilder MapGetOpenApiContractsEndpoint(this IEndpointRouteBuilder builder)
    {
        builder.MapGet(NotificationsEndpoints.OpenApiContractEndpoints.GetOpenApiContracts,
                () => Results.NoContent())
            .Produces<OpenApiResponse>()
            .HasApiVersion(1.0)
            .WithName(NotificationsEndpoints.GetOpenApiName(nameof(NotificationsEndpoints.OpenApiContractEndpoints.GetOpenApiContracts)));

        return builder;
    }
}
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Nexticz.Module.Mmo.Washing.Contracts.WashingStates;
using Nexticz.Module.Mmo.Washing.Application.WashingStates.Queries.GetWashingStateResponse;

namespace Nexticz.Module.Mmo.Washing.Presentation.WashingStates;

internal static class GetWashingStateEndpoint
{
    public static IEndpointRouteBuilder MapGetWashingStateEndpoint(this IEndpointRouteBuilder builder)
    {
        builder.MapGet(WashingEndpoints.WashingStateEndpoints.GetWashingState,
                async (
                    ISender sender,
                    CancellationToken cancellationToken
                ) =>
                {
                    var result = 
                        await sender.Send(
                            new GetWashingStateResponseQuery(), cancellationToken);

                    return Results.Ok(result);
                })
            .Produces<WashingStateResponse>()
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .HasApiVersion(1.0)
            .WithName(WashingEndpoints.GetOpenApiName(nameof(WashingEndpoints.WashingStateEndpoints.GetWashingState)));

        return builder;
    }
}
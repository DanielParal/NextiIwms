using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Nexticz.Module.Portal.Application.Seeds.Orchestrators;

namespace Nexticz.Module.Portal.Presentation.Seeds;

internal static class CreateSeedEndpoint
{
    public static IEndpointRouteBuilder MapCreateSeed(this IEndpointRouteBuilder builder)
    {
        builder.MapPost(PortalEndpoints.SeedEndpoints.CreateSeed,
                async (
                    CancellationToken cancellationToken,
                    [FromServices] ISeedOrchestrator seedOrchestrator
                ) =>
                {
                    await seedOrchestrator.SeedAsync(cancellationToken);
                    return Results.NoContent();
                })
            .Produces<object>(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .HasApiVersion(1.0)
            .WithName(PortalEndpoints.GetOpenApiName(nameof(PortalEndpoints.SeedEndpoints.CreateSeed)));

        return builder;
    }
}
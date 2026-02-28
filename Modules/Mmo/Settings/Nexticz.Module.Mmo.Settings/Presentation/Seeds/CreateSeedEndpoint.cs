using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Nexticz.Module.Mmo.Settings.Application.Seeds.Commands.CreateSeed;

namespace Nexticz.Module.Mmo.Settings.Presentation.Seeds;

internal static class CreateSeedEndpoint
{
    public static IEndpointRouteBuilder MapCreateSeed(this IEndpointRouteBuilder builder)
    {
        builder.MapPost(SettingsEndpoints.SeedEndpoints.CreateSeed,
                async (
                    ISender mediator,
                    CancellationToken cancellationToken
                ) =>
                {
                    await mediator.Send(new CreateSeedCommand(), cancellationToken);
                    return Results.Ok();
                })
            .Produces<object>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .HasApiVersion(1.0)
            .WithName(SettingsEndpoints.GetOpenApiName(nameof(SettingsEndpoints.SeedEndpoints.CreateSeed)));

        return builder;
    }
}
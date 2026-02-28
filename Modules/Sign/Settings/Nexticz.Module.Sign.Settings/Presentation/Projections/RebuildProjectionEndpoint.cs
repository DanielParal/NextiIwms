using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Nexticz.Module.Sign.Settings.Application.Projections.Commands;

namespace Nexticz.Module.Sign.Settings.Presentation.Projections;

internal static class RebuildProjectionEndpoint
{
    public static IEndpointRouteBuilder MapRebuildProjectionEndpoint(this IEndpointRouteBuilder builder)
    {
        builder.MapPost(SettingsEndpoints.ProjectionEndpoints.RebuildProjection,
                async (
                    string projectionTypeString,
                    ISender sender,
                    CancellationToken cancellationToken
                ) =>
                {
                    var success = await sender.Send(new RebuildProjectionCommand(projectionTypeString), cancellationToken);
                    
                    return success ? Results.Ok() : Results.BadRequest();
                })
            .Produces<object>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .HasApiVersion(1.0)
            .WithName(SettingsEndpoints.GetOpenApiName(nameof(SettingsEndpoints.ProjectionEndpoints.RebuildProjection)));

        return builder;
    }
}
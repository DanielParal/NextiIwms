using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Nexticz.Lib.Shared.Errors.Models;
using Nexticz.Lib.Shared.Helpers;
using Nexticz.Module.Portal.Application.Modules.Commands.DeleteModule;

namespace Nexticz.Module.Portal.Presentation.Modules;

internal static class DeleteModuleEndpoint
{
    public static IEndpointRouteBuilder MapDeleteModuleEndpoint(this IEndpointRouteBuilder builder)
    {
        builder.MapDelete(PortalEndpoints.ModuleEndpoints.DeleteModule,
                async (
                    Guid id, 
                    ISender sender,
                    CancellationToken cancellationToken) =>
                {
                    var result = 
                        await sender.Send(new DeleteModuleCommand(id), cancellationToken);

                    return result.Match(
                        _ => Results.NoContent(),
                        ResultsHelper.Problem);
                })
            .Produces<object>(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces<ApiErrorResponse>(StatusCodes.Status422UnprocessableEntity)
            .HasApiVersion(1.0)
            .WithName(PortalEndpoints.GetOpenApiName(nameof(PortalEndpoints.ModuleEndpoints.DeleteModule)));

        return builder;
    }
}
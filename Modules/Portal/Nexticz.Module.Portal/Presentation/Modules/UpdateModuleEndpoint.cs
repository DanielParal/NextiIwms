using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Nexticz.Module.Portal.Contracts.Modules;
using Nexticz.Lib.Shared.Errors.Models;
using Nexticz.Lib.Shared.Helpers;
using Nexticz.Module.Portal.Application.Modules.Commands.UpdateModule;

namespace Nexticz.Module.Portal.Presentation.Modules;

internal static class UpdateModuleEndpoint
{
    public static IEndpointRouteBuilder MapUpdateModuleEndpoint(this IEndpointRouteBuilder builder)
    {
        builder.MapPut(PortalEndpoints.ModuleEndpoints.UpdateModule,
                async (
                    Guid id, 
                    UpdateModuleRequest request, 
                    ISender sender,
                    CancellationToken cancellationToken) =>
                {
                    var result = 
                        await sender.Send(new UpdateModuleCommand(id, request), cancellationToken);

                    return result.Match(
                        _ => Results.NoContent(),
                        ResultsHelper.Problem);
                })
            .Produces<object>(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces<ApiErrorResponse>(StatusCodes.Status422UnprocessableEntity)
            .HasApiVersion(1.0)
            .WithName(PortalEndpoints.GetOpenApiName(nameof(PortalEndpoints.ModuleEndpoints.UpdateModule)));
        
        return builder;
    }
}
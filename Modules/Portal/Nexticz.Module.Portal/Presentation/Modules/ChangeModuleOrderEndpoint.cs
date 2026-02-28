using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Nexticz.Module.Portal.Contracts.Modules;
using Nexticz.Lib.Shared.Errors.Models;
using Nexticz.Lib.Shared.Helpers;
using Nexticz.Module.Portal.Application.Modules.Commands.ChangeModuleOrder;

namespace Nexticz.Module.Portal.Presentation.Modules;

public static class ChangeModuleOrderEndpoint
{
    public static IEndpointRouteBuilder MapChangeModuleOrderEndpoint(this IEndpointRouteBuilder builder)
    {
        builder.MapPost(PortalEndpoints.ModuleEndpoints.ChangeModuleOrder,
                async (
                    Guid id,
                    ChangeModuleOrderRequest changeModuleOrderRequest, 
                    ISender sender,
                    CancellationToken cancellationToken) =>
                {
                    var result = 
                        await sender.Send(new ChangeModuleOrderCommand(id, changeModuleOrderRequest.ToOrder), cancellationToken);

                    return result.Match(
                        _ => Results.NoContent(),
                        ResultsHelper.Problem);  
                })
            .Produces<object>(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces<ApiErrorResponse>(StatusCodes.Status422UnprocessableEntity)
            .HasApiVersion(1.0)
            .WithName(PortalEndpoints.GetOpenApiName(nameof(PortalEndpoints.ModuleEndpoints.ChangeModuleOrder)));

        return builder;
    }
}
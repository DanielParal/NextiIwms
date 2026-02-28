using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Nexticz.Module.Portal.Contracts.Modules;
using Nexticz.Lib.Shared.Helpers;
using Nexticz.Module.Portal.Application.Modules.Queries.GetModuleById;

namespace Nexticz.Module.Portal.Presentation.Modules;

internal static class GetModuleByIdEndpoint
{
    public static IEndpointRouteBuilder MapGetModuleByIdEndpoint(this IEndpointRouteBuilder builder)
    {
        builder.MapGet(PortalEndpoints.ModuleEndpoints.GetModuleById,
                async (
                    Guid id, 
                    ISender sender,
                    CancellationToken cancellationToken
                ) =>
                {
                    var result = 
                        await sender.Send(new GetModuleByIdQuery(id), cancellationToken);
                    
                    return result.Match(
                        module => Results.Ok(ModuleResponseFactory.Create(module)),
                        ResultsHelper.Problem);
                })
            .Produces<ModuleResponse>()
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status404NotFound)
            .HasApiVersion(1.0)
            .WithName(PortalEndpoints.GetOpenApiName(nameof(PortalEndpoints.ModuleEndpoints.GetModuleById)));

        return builder;
    }
}
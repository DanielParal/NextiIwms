using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Nexticz.Module.Portal.Contracts.Modules;
using Nexticz.Lib.Shared.Errors.Models;
using Nexticz.Lib.Shared.Helpers;
using Nexticz.Module.Portal.Application.Modules.Commands.CreateModule;

namespace Nexticz.Module.Portal.Presentation.Modules;

internal static class CreateModuleEndpoint
{
    public static IEndpointRouteBuilder MapCreateModuleEndpoint(this IEndpointRouteBuilder builder)
    {
        builder.MapPost(PortalEndpoints.ModuleEndpoints.CreateModule,
                async (
                    CreateModuleRequest request,
                    ISender sender,
                    CancellationToken cancellationToken) =>
                {
                    var result = 
                        await sender.Send(new CreateModuleCommand(request), 
                            cancellationToken);

                    return result.Match(
                        module => Results.Created(
                            $"{PortalEndpoints.ModuleEndpoints.GetModules}/{module.Id}",
                            ModuleResponseFactory.Create(module)),
                        ResultsHelper.Problem);
                })
            .Produces<ModuleResponse>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces<ApiErrorResponse>(StatusCodes.Status422UnprocessableEntity)
            .HasApiVersion(1.0)
            .WithName(PortalEndpoints.GetOpenApiName(nameof(PortalEndpoints.ModuleEndpoints.CreateModule)));

        return builder;
    }
}
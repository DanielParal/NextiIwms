using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Nexticz.Lib.Shared.Errors.Models;
using Nexticz.Lib.Shared.Helpers;
using Nexticz.Module.Mmo.Washing.Application.WashingMachineSoses.Commands.ResolveSos;


namespace Nexticz.Module.Mmo.Washing.Presentation.WashingMachineSoses;

internal static class ResolveSosEndpoint
{
    public static IEndpointRouteBuilder MapResolveSosEndpoint(this IEndpointRouteBuilder builder)
    {
        builder.MapPost(WashingEndpoints.WashingMachineSosEndpoints.ResolveSos,
                async (
                    string washingMachineCode,
                    ISender sender,
                    CancellationToken cancellationToken
                ) =>
                {
                    var result = 
                        await sender.Send(new ResolveSosCommand(washingMachineCode), cancellationToken);
                    
                    return result.Match(
                        _ => Results.Ok(),
                        ResultsHelper.Problem);
                })
            .Produces<object>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces<ApiErrorResponse>(StatusCodes.Status422UnprocessableEntity)
            .HasApiVersion(1.0)
            .WithName(WashingEndpoints.GetOpenApiName(nameof(WashingEndpoints.WashingMachineSosEndpoints.ResolveSos)));

        return builder;
    }
}
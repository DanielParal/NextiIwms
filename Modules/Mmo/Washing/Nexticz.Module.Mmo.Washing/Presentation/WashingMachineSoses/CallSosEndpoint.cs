using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Nexticz.Lib.Shared.Errors.Models;
using Nexticz.Lib.Shared.Helpers;
using Nexticz.Module.Mmo.Washing.Application.WashingMachineSoses.Commands.CallSos;


namespace Nexticz.Module.Mmo.Washing.Presentation.WashingMachineSoses;

internal static class CallSosEndpoint
{
    public static IEndpointRouteBuilder MapCallSosEndpoint(this IEndpointRouteBuilder builder)
    {
        builder.MapPost(WashingEndpoints.WashingMachineSosEndpoints.CallSos,
                async (
                    string washingMachineCode,
                    ISender sender,
                    CancellationToken cancellationToken
                ) =>
                {
                    var result = 
                        await sender.Send(new CallSosCommand(washingMachineCode), cancellationToken);
                    
                    return result.Match(
                        _ => Results.Ok(),
                        ResultsHelper.Problem);
                })
            .Produces<object>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces<ApiErrorResponse>(StatusCodes.Status422UnprocessableEntity)
            .HasApiVersion(1.0)
            .WithName(WashingEndpoints.GetOpenApiName(nameof(WashingEndpoints.WashingMachineSosEndpoints.CallSos)));

        return builder;
    }
}
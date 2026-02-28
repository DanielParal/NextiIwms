using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Nexticz.Lib.Shared.Errors.Models;
using Nexticz.Lib.Shared.Helpers;
using Nexticz.Module.Mmo.Washing.Application.LastEnteredWorkerOnLines.Commands.LeaveLine;


namespace Nexticz.Module.Mmo.Washing.Presentation.LastEnteredWorkerOnLines;

internal static class LeaveLineEndpoint
{
    public static IEndpointRouteBuilder MapLeaveLineEndpoint(this IEndpointRouteBuilder builder)
    {
        builder.MapPost(WashingEndpoints.LastEnteredWorkerOnLineEndpoints.LeaveLine,
                async (
                    string lineCode,
                    ISender sender,
                    CancellationToken cancellationToken) =>
                {
                    var result = 
                        await sender.Send(new LeaveLineCommand(lineCode), cancellationToken);
                    
                    return result.Match(
                        _ => Results.Ok(),
                        ResultsHelper.Problem);
                })
            .Produces<object>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces<ApiErrorResponse>(StatusCodes.Status422UnprocessableEntity)
            .HasApiVersion(1.0)
            .WithName(WashingEndpoints.GetOpenApiName(nameof(WashingEndpoints.LastEnteredWorkerOnLineEndpoints.LeaveLine)));

        return builder;
    }
}
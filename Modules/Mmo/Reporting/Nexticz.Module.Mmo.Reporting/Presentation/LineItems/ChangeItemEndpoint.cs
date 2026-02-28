using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Nexticz.Module.Mmo.Reporting.Contracts.LineItems;
using Nexticz.Lib.Shared.Errors.Models;
using Nexticz.Lib.Shared.Helpers;
using Nexticz.Module.Mmo.Reporting.Application.LineItems.Orchestrators.ChangeItem;


namespace Nexticz.Module.Mmo.Reporting.Presentation.LineItems;

internal static class ChangeItemEndpoint
{
    public static IEndpointRouteBuilder MapChangeItemEndpoint(this IEndpointRouteBuilder builder)
    {
        builder.MapPost(ReportingEndpoints.LineItemEndpoints.ChangeItem,
                async (
                    Guid id,
                    ChangeItemRequest request,
                    CancellationToken cancellationToken,
                    [FromServices] IChangeItemOrchestrator changeItemOrchestrator
                ) =>
                {
                    var result = 
                        await changeItemOrchestrator.ChangeItemAsync(
                            id, request.CutTime, request.Type, request.InactivityReasonId, cancellationToken);;
                    
                    return result.Match(
                        _ => Results.Ok(),
                        ResultsHelper.Problem);
                })
            .Produces<object>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces<ApiErrorResponse>(StatusCodes.Status422UnprocessableEntity)
            .HasApiVersion(1.0)
            .WithName(ReportingEndpoints.GetOpenApiName(nameof(ReportingEndpoints.LineItemEndpoints.ChangeItem)));

        return builder;
    }
}
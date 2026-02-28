using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Nexticz.Module.Mmo.Reporting.Contracts.LineItems;
using Nexticz.Lib.Shared.Errors.Models;
using Nexticz.Lib.Shared.Helpers;
using Nexticz.Module.Mmo.Reporting.Application.LineItems.Orchestrators.ChangeComment;

namespace Nexticz.Module.Mmo.Reporting.Presentation.LineItems;

internal static class ChangeCommentEndpoint
{
    public static IEndpointRouteBuilder MapChangeCommentEndpoint(this IEndpointRouteBuilder builder)
    {
        builder.MapPost(ReportingEndpoints.LineItemEndpoints.ChangeComment,
                async (
                    Guid id,
                    ChangeCommentRequest request,
                    CancellationToken cancellationToken,
                    [FromServices] IChangeCommentOrchestrator orchestrator
                ) =>
                {
                    var result = 
                        await orchestrator.ChangeCommentAsync(id, request.Comment, cancellationToken);
                    
                    return result.Match(
                        _ => Results.Ok(),
                        ResultsHelper.Problem);
                })
            .Produces<object>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces<ApiErrorResponse>(StatusCodes.Status422UnprocessableEntity)
            .HasApiVersion(1.0)
            .WithName(ReportingEndpoints.GetOpenApiName(nameof(ReportingEndpoints.LineItemEndpoints.ChangeComment)));

        return builder;
    }
}
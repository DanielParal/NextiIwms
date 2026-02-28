using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Nexticz.Module.Sign.DocumentManager.Contracts.Emails;
using Nexticz.Lib.Shared.Errors.Models;
using Nexticz.Lib.Shared.Helpers;
using Nexticz.Module.Sign.DocumentManager.Application.Emails.Orchestrators;

namespace Nexticz.Module.Sign.DocumentManager.Presentation.Emails;

internal static class SendEmailsEndpoint
{
    public static IEndpointRouteBuilder MapSendEmailsEndpoint(this IEndpointRouteBuilder builder)
    {
        builder.MapPost(DocumentManagerEndpoints.EmailEndpoints.SendEmails,
                async (
                    SendEmailsRequest request,
                    ISendEmailsOrchestrator orchestrator,
                    CancellationToken cancellationToken) =>
                {
                    var result = 
                        await orchestrator.OrchestrateAsync(request.Recipients, request.SendEmailJobs, 
                            cancellationToken);
                    
                    return result.Match(
                        _ => Results.Ok(),
                        ResultsHelper.Problem);
                })
            .Produces<object>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces<ApiErrorResponse>(StatusCodes.Status422UnprocessableEntity)
            .HasApiVersion(1.0)
            .WithName(DocumentManagerEndpoints.GetOpenApiName(nameof(DocumentManagerEndpoints.EmailEndpoints.SendEmails)));

        return builder;
    }
}
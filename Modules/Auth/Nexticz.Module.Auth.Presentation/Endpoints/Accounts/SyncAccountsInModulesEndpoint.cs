using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Nexticz.Module.Auth.Application.Accounts.Common.Models;
using Nexticz.Module.Auth.Application.Accounts.Orchestrators;
using Nexticz.Module.Auth.Application.Accounts.Queries.GetAccounts;
using Nexticz.Module.Auth.Application.MasstransitPublishers;
using Nexticz.Module.Auth.Contracts.Accounts;
using Nexticz.Lib.Shared.Errors.Models;
using Nexticz.Lib.Shared.Helpers;


namespace Nexticz.Module.Auth.Presentation.Endpoints.Accounts;

internal static class SyncAccountsInModulesEndpoint
{
    public static IEndpointRouteBuilder MapSyncAccountsInModules(this IEndpointRouteBuilder builder)
    {
        builder.MapPost(ApiEndpoints.Accounts.SyncAccountsInModules, 
                async (ISyncAccountOrchestrator syncOrchestrator, CancellationToken cancellationToken) =>
                {
                    var result = await syncOrchestrator.OrchestrateAsync(cancellationToken);
                    
                    return result.Match(
                        _ => Results.NoContent(),
                        ResultsHelper.Problem);
                })
            .HasApiVersion(1.0)
            .Produces(StatusCodes.Status204NoContent)
            .Produces<ApiErrorResponse>(StatusCodes.Status422UnprocessableEntity)
            .Produces(StatusCodes.Status404NotFound)
            .WithName(nameof(ApiEndpoints.Accounts.SyncAccountsInModules));

        return builder;
    }
}
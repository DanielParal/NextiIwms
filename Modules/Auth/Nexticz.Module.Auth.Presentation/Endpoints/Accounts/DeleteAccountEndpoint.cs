using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Nexticz.Module.Auth.Application.Accounts.Commands.DeleteAccount;
using Nexticz.Lib.Shared.Helpers;

namespace Nexticz.Module.Auth.Presentation.Endpoints.Accounts;

public static class DeleteAccountEndpoint
{
    public static IEndpointRouteBuilder MapDeleteAccount(this IEndpointRouteBuilder builder)
    {
        builder.MapDelete(ApiEndpoints.Accounts.DeleteAccount, async (Guid id, IMediator mediatr, CancellationToken cancellationToken) =>
            {
                var result = await mediatr.Send(new DeleteAccountCommand(id), cancellationToken);
                
                return result.Match(
                    _ => Results.NoContent(),
                    ResultsHelper.Problem);
                
            }).HasApiVersion(1.0)
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound)
            .WithName(nameof(ApiEndpoints.Accounts.DeleteAccount));
        
        return builder;
    }
}
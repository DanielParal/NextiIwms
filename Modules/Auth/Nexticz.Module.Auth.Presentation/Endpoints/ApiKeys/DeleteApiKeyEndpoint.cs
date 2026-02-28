using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Nexticz.Module.Auth.Application.ApiKeys.Commands.DeleteApiKey;
using Nexticz.Lib.Shared.Errors.Models;
using Nexticz.Lib.Shared.Helpers;


namespace Nexticz.Module.Auth.Presentation.Endpoints.ApiKeys;

public static class DeleteApiKeyEndpoint
{
    public static IEndpointRouteBuilder MapDeleteApiKey(this IEndpointRouteBuilder builder)
    {
        builder.MapDelete(ApiEndpoints.ApiKeys.DeleteApiKey, async (Guid id, ISender mediatr,
            CancellationToken cancellationToken) =>
        {
            var command = new DeleteApiKeyCommand { Id = id };
            var result = await mediatr.Send(command, cancellationToken);

            return result.Match(
                Results.Ok,
                ResultsHelper.Problem);
        })
        .Produces(StatusCodes.Status204NoContent)
        .Produces<ApiErrorResponse>(StatusCodes.Status404NotFound)
        .HasApiVersion(1.0)
        .WithName(nameof(ApiEndpoints.ApiKeys.DeleteApiKey));;
        
        return builder;
    }
}
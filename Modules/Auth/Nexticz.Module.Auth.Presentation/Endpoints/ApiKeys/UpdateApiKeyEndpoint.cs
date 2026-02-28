using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Nexticz.Module.Auth.Application.ApiKeys.Commands.UpdateApiKey;
using Nexticz.Module.Auth.Contracts.ApiKeys;
using Nexticz.Lib.Shared.Errors.Models;
using Nexticz.Lib.Shared.Helpers;


namespace Nexticz.Module.Auth.Presentation.Endpoints.ApiKeys;

public static class UpdateApiKeyEndpoint
{
    public static IEndpointRouteBuilder MapUpdateApiKey(this IEndpointRouteBuilder builder)
    {
        builder.MapPut(ApiEndpoints.ApiKeys.UpdateApiKey, async (Guid id, UpdateApiKeyRequest updateApiKeyRequest, ISender mediatr, CancellationToken cancellationToken) =>
        {
            var command = new UpdateApiKeyCommand(id, updateApiKeyRequest);
            var result = await mediatr.Send(command, cancellationToken);

            return result.Match(
                _ => Results.NoContent(),
                ResultsHelper.Problem);
        })
        .Produces(StatusCodes.Status204NoContent)
        .Produces<ApiErrorResponse>(StatusCodes.Status404NotFound)
        .HasApiVersion(1.0)
        .WithName(nameof(ApiEndpoints.ApiKeys.UpdateApiKey));
        
        return builder;
    }
}
using Azure.Core;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Nexticz.Module.Auth.Application.ApiKeys.Commands.CreateApiKey;
using Nexticz.Module.Auth.Contracts.ApiKeys;
using Nexticz.Lib.Shared.Helpers;

namespace Nexticz.Module.Auth.Presentation.Endpoints.ApiKeys;

public static class CreateApiKeyEndpoint
{
    public static IEndpointRouteBuilder MapCreateApiKey(this IEndpointRouteBuilder builder)
    {
        builder.MapPost(ApiEndpoints.ApiKeys.CreateApiKey, async (CreateApiKeyRequest request,
            ISender mediatr, CancellationToken cancellationToken) =>
            {
                var command = new CreateApiKeyCommand { CreateApiKeyRequest = request};
                var result = await mediatr.Send(command, cancellationToken);

                return result.Match(
                    Results.Ok,
                    ResultsHelper.Problem);
            })
        .HasApiVersion(1.0)
        .WithName(nameof(ApiEndpoints.ApiKeys.CreateApiKey));
        
        return builder;
    }
}
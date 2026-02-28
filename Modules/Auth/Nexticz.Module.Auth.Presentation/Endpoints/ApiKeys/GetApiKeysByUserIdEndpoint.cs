using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Nexticz.Module.Auth.Application.ApiKeys.Queries.GetApiKeyById;
using Nexticz.Module.Auth.Contracts.ApiKeys;
using Nexticz.Module.Auth.Presentation.Endpoints.ApiKeys.Mappers;
using Nexticz.Lib.Shared.Helpers;

namespace Nexticz.Module.Auth.Presentation.Endpoints.ApiKeys;

public static class GetApiKeysByUserIdEndpoint
{
    public static IEndpointRouteBuilder MapGetApiKeysByUserId(this IEndpointRouteBuilder builder)
    {
        builder.MapGet(ApiEndpoints.ApiKeys.GetApiKeysByUserId, async (Guid userId,
                ISender mediatr, CancellationToken cancellationToken) =>
            {
                var query = new GetApiKeysByUserIdQuery(userId);
                var result = await mediatr.Send(query, cancellationToken);

                return result.Match(
                    value => Results.Ok(value.MapToApiKeysResponse()),
                    ResultsHelper.Problem);
            })
            .Produces<List<ApiKeyResponse>>()
            .HasApiVersion(1.0)
            .WithName(nameof(ApiEndpoints.ApiKeys.GetApiKeysByUserId));

        return builder;
    }
}
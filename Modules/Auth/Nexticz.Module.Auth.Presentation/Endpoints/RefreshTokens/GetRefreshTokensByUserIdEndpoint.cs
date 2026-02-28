using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Nexticz.Module.Auth.Application.RefreshTokens.Queries.GetRefreshTokensByUseId;
using Nexticz.Module.Auth.Contracts.RefreshTokens;
using Nexticz.Module.Auth.Presentation.Endpoints.RefreshTokens.Mappers;
using Nexticz.Lib.Shared.Helpers;

namespace Nexticz.Module.Auth.Presentation.Endpoints.RefreshTokens;

public static class GetRefreshTokensByUserIdEndpoint
{
    public static IEndpointRouteBuilder MapGetRefreshTokensByUserId(this IEndpointRouteBuilder builder)
    {
        builder.MapGet(ApiEndpoints.RefreshTokens.GetRefreshTokensByUserId,
                async (Guid userId,
                    ISender mediatr, CancellationToken cancellationToken) =>
                {
                    var query = new GetRefreshTokensByUserIdQuery(userId);
                    var result = await mediatr.Send(query, cancellationToken);

                    return result.Match(
                        tokens => Results.Ok(tokens.MapToRefreshTokensResponse()),
                        ResultsHelper.Problem);
                })
            .Produces<List<RefreshTokenResponse>>()
            .HasApiVersion(1.0)
            .WithName(nameof(ApiEndpoints.RefreshTokens.GetRefreshTokensByUserId));

        return builder;
    }
}
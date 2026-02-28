using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Nexticz.Module.Vh.Application.BandRewards.Queries.GetBandRewardById;
using Nexticz.Module.Vh.Contracts.BandRewards;
using Nexticz.Lib.Shared.Errors.Models;
using Nexticz.Lib.Shared.Helpers;


namespace Nexticz.Module.Vh.Presentation.Endpoints.BandRewards;

public static class GetBandRewardByIdEndpoint
{
    public static IEndpointRouteBuilder MapGetBandRewardById(this IEndpointRouteBuilder builder)
    {
        builder.MapGet(ApiEndpoints.BandRewards.GetBandRewardById,
                async (Guid id, ISender mediatr, CancellationToken cancellationToken) =>
                {
                    var query = new GetBandRewardByIdQuery { Id = id };
                    var result = await mediatr.Send(query, cancellationToken);

                    return result.Match(
                        Results.Ok,
                        ResultsHelper.Problem);
                })
            .Produces<BandRewardResponse>()
            .Produces<ApiErrorResponse>(StatusCodes.Status404NotFound)
            .HasApiVersion(1.0)
            .WithName(nameof(ApiEndpoints.BandRewards.GetBandRewardById));

        return builder;
    }
}
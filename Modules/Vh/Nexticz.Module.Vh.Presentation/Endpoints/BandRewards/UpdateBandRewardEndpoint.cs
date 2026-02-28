using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Nexticz.Module.Vh.Application.BandRewards.Commands.UpdateBandReward;
using Nexticz.Module.Vh.Contracts.BandRewards;
using Nexticz.Lib.Shared.Errors.Models;
using Nexticz.Lib.Shared.Helpers;


namespace Nexticz.Module.Vh.Presentation.Endpoints.BandRewards;

public static class UpdateBandRewardEndpoint
{
    public static IEndpointRouteBuilder MapUpdateBandReward(this IEndpointRouteBuilder builder)
    {
        builder.MapPut(ApiEndpoints.BandRewards.UpdateBandReward,
                async (Guid id, UpdateBandRewardRequest updateBandRewardRequest, ISender mediatr,
                    CancellationToken cancellationToken) =>
                {
                    var command = new UpdateBandRewardCommand
                        { Id = id, UpdateBandRewardRequest = updateBandRewardRequest };
                    var result = await mediatr.Send(command, cancellationToken);

                    return result.Match(
                        _ => Results.NoContent(),
                        ResultsHelper.Problem);
                })
            .Produces(StatusCodes.Status204NoContent)
            .Produces<ApiErrorResponse>(StatusCodes.Status404NotFound)
            .HasApiVersion(1.0)
            .WithName(nameof(ApiEndpoints.BandRewards.UpdateBandReward));

        return builder;
    }
}
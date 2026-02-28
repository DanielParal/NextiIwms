using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Nexticz.Module.Vh.Application.BandRewards.Commands.DeleteBandReward;
using Nexticz.Lib.Shared.Errors.Models;
using Nexticz.Lib.Shared.Helpers;


namespace Nexticz.Module.Vh.Presentation.Endpoints.BandRewards;

public static class DeleteBandRewardEndpoint
{
    public static IEndpointRouteBuilder MapDeleteBandReward(this IEndpointRouteBuilder builder)
    {
        builder.MapDelete(ApiEndpoints.BandRewards.DeleteBandReward,
                async (Guid id, ISender mediatr, CancellationToken cancellationToken) =>
                {
                    var command = new DeleteBandRewardCommand { Id = id };
                    var result = await mediatr.Send(command, cancellationToken);

                    return result.Match(
                        _ => Results.NoContent(),
                        ResultsHelper.Problem);
                })
            .Produces(StatusCodes.Status204NoContent)
            .Produces<ApiErrorResponse>(StatusCodes.Status404NotFound)
            .HasApiVersion(1.0)
            .WithName(nameof(ApiEndpoints.BandRewards.DeleteBandReward));

        return builder;
    }
}
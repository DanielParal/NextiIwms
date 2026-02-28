using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Nexticz.Module.Vh.Application.BandRewards.Commands.CreateBandReward;
using Nexticz.Module.Vh.Contracts.BandRewards;
using Nexticz.Lib.Shared.Errors.Models;
using Nexticz.Lib.Shared.Helpers;


namespace Nexticz.Module.Vh.Presentation.Endpoints.BandRewards;

public static class CreateBandRewardEndpoint
{
    public static IEndpointRouteBuilder MapCreateBandReward(this IEndpointRouteBuilder builder)
    {
        builder.MapPost(ApiEndpoints.BandRewards.CreateBandReward,
                async (CreateBandRewardRequest createBandRewardRequest, ISender mediatr,
                    CancellationToken cancellationToken) =>
                {
                    var command = new CreateBandRewardCommand { CreateBandRewardRequest = createBandRewardRequest };
                    var result = await mediatr.Send(command, cancellationToken);

                    return result.Match(
                        _ => Results.NoContent(),
                        ResultsHelper.Problem);
                })
            .Produces(StatusCodes.Status204NoContent)
            .Produces<ApiErrorResponse>()
            .HasApiVersion(1.0)
            .WithName(nameof(ApiEndpoints.BandRewards.CreateBandReward));

        return builder;
    }
}
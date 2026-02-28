using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Nexticz.Module.Vh.Application.LoadingActionsNdas.Commands.CreateLoadingActionsNda;
using Nexticz.Module.Vh.Contracts.LoadingActionsNdas;
using Nexticz.Lib.Shared.Errors.Models;
using Nexticz.Lib.Shared.Helpers;


namespace Nexticz.Module.Vh.Presentation.Endpoints.LoadingActionsNdas;

public static class CreateLoadingActionsNdaEndpoint
{
    public static IEndpointRouteBuilder MapCreateLoadingActionsNda(this IEndpointRouteBuilder builder)
    {
        builder.MapPost(ApiEndpoints.LoadingActionsNdas.CreateLoadingActionsNda,
                async (CreateLoadingActionsNdaRequest createLoadingActionsNdaRequest, ISender mediatr,
                    CancellationToken cancellationToken) =>
                {
                    var command = new CreateLoadingActionsNdaCommand
                        { CreateLoadingActionsNdaRequest = createLoadingActionsNdaRequest };
                    var result = await mediatr.Send(command, cancellationToken);

                    return result.Match(
                        _ => Results.NoContent(),
                        ResultsHelper.Problem);
                })
            .Produces(StatusCodes.Status204NoContent)
            .Produces<ApiErrorResponse>()
            .HasApiVersion(1.0)
            .WithName(nameof(ApiEndpoints.LoadingActionsNdas.CreateLoadingActionsNda));

        return builder;
    }
}
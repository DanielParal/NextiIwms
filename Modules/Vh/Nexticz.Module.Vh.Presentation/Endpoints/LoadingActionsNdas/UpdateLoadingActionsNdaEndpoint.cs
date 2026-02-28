using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Nexticz.Module.Vh.Application.LoadingActionsNdas.Commands.UpdateLoadingActionsNda;
using Nexticz.Module.Vh.Contracts.LoadingActionsNdas;
using Nexticz.Lib.Shared.Errors.Models;
using Nexticz.Lib.Shared.Helpers;


namespace Nexticz.Module.Vh.Presentation.Endpoints.LoadingActionsNdas;

public static class UpdateLoadingActionsNdaEndpoint
{
    public static IEndpointRouteBuilder MapUpdateLoadingActionsNda(this IEndpointRouteBuilder builder)
    {
        builder.MapPut(ApiEndpoints.LoadingActionsNdas.UpdateLoadingActionsNda,
                async (Guid id, UpdateLoadingActionsNdaRequest updateLoadingActionsNdaRequest, ISender mediatr,
                    CancellationToken cancellationToken) =>
                {
                    var command = new UpdateLoadingActionsNdaCommand
                        { Id = id, UpdateLoadingActionsNdaRequest = updateLoadingActionsNdaRequest };
                    var result = await mediatr.Send(command, cancellationToken);

                    return result.Match(
                        _ => Results.NoContent(),
                        ResultsHelper.Problem);
                })
            .Produces(StatusCodes.Status204NoContent)
            .Produces<ApiErrorResponse>(StatusCodes.Status404NotFound)
            .HasApiVersion(1.0)
            .WithName(nameof(ApiEndpoints.LoadingActionsNdas.UpdateLoadingActionsNda));

        return builder;
    }
}
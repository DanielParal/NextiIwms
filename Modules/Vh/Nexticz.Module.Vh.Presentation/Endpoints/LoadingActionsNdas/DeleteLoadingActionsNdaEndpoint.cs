using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Nexticz.Module.Vh.Application.LoadingActionsNdas.Commands.DeleteLoadingActionsNda;
using Nexticz.Lib.Shared.Errors.Models;
using Nexticz.Lib.Shared.Helpers;


namespace Nexticz.Module.Vh.Presentation.Endpoints.LoadingActionsNdas;

public static class DeleteLoadingActionsNdaEndpoint
{
    public static IEndpointRouteBuilder MapDeleteLoadingActionsNda(this IEndpointRouteBuilder builder)
    {
        builder.MapDelete(ApiEndpoints.LoadingActionsNdas.DeleteLoadingActionsNda,
                async (Guid id, ISender mediatr, CancellationToken cancellationToken) =>
                {
                    var command = new DeleteLoadingActionsNdaCommand { Id = id };
                    var result = await mediatr.Send(command, cancellationToken);

                    return result.Match(
                        _ => Results.NoContent(),
                        ResultsHelper.Problem);
                })
            .Produces(StatusCodes.Status204NoContent)
            .Produces<ApiErrorResponse>(StatusCodes.Status404NotFound)
            .HasApiVersion(1.0)
            .WithName(nameof(ApiEndpoints.LoadingActionsNdas.DeleteLoadingActionsNda));

        return builder;
    }
}
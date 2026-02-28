using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Nexticz.Module.Vh.Application.LoadingActionsNdas.Queries.GetLoadingActionsNdaById;
using Nexticz.Module.Vh.Contracts.LoadingActionsNdas;
using Nexticz.Lib.Shared.Errors.Models;
using Nexticz.Lib.Shared.Helpers;


namespace Nexticz.Module.Vh.Presentation.Endpoints.LoadingActionsNdas;

public static class GetLoadingActionsNdaByIdEndpoint
{
    public static IEndpointRouteBuilder MapGetLoadingActionsNdaById(this IEndpointRouteBuilder builder)
    {
        builder.MapGet(ApiEndpoints.LoadingActionsNdas.GetLoadingActionsNdaById,
                async (Guid id, ISender mediatr, CancellationToken cancellationToken) =>
                {
                    var query = new GetLoadingActionsNdaByIdQuery { Id = id };
                    var result = await mediatr.Send(query, cancellationToken);

                    return result.Match(
                        Results.Ok,
                        ResultsHelper.Problem);
                })
            .Produces<LoadingActionsNdaResponse>()
            .Produces<ApiErrorResponse>(StatusCodes.Status404NotFound)
            .HasApiVersion(1.0)
            .AllowAnonymous()
            .WithName(nameof(ApiEndpoints.LoadingActionsNdas.GetLoadingActionsNdaById));

        return builder;
    }
}
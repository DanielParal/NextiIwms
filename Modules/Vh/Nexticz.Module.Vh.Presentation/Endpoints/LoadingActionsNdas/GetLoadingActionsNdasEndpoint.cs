using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Nexticz.Module.Vh.Application.LoadingActionsNdas.Common.Models;
using Nexticz.Module.Vh.Application.LoadingActionsNdas.Queries.GetLoadingActionsNdas;
using Nexticz.Module.Vh.Contracts.LoadingActionsNdas;
using Nexticz.Lib.Shared.DevExtreme;
using Nexticz.Lib.Shared.Helpers;


namespace Nexticz.Module.Vh.Presentation.Endpoints.LoadingActionsNdas;

public static class GetLoadingActionsNdasEndpoint
{
    public static IEndpointRouteBuilder MapGetLoadingActionsNdas(this IEndpointRouteBuilder builder)
    {
        builder.MapGet(ApiEndpoints.LoadingActionsNdas.GetLoadingActionsNdas,
                async ([AsParameters] LoadingActionsNdasFilteringParams filteringParams, ISender mediatr,
                    CancellationToken cancellationToken) =>
                {
                    var query = new GetLoadingActionsNdasQuery { FilteringParams = filteringParams };
                    var result = await mediatr.Send(query, cancellationToken);

                    return result.Match(
                        Results.Ok,
                        ResultsHelper.Problem);
                })
            .Produces<FilteredResult<LoadingActionsNdaResponse>>()
            .HasApiVersion(1.0)
            .WithName(nameof(ApiEndpoints.LoadingActionsNdas.GetLoadingActionsNdas));

        return builder;
    }
}
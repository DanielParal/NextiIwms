using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Nexticz.Module.Vh.Application.Assortments.Common.Models;
using Nexticz.Module.Vh.Application.Assortments.Queries.GetAssortments;
using Nexticz.Module.Vh.Contracts.Assortments;
using Nexticz.Lib.Shared.DevExtreme;
using Nexticz.Lib.Shared.Helpers;


namespace Nexticz.Module.Vh.Presentation.Endpoints.Assortments;

public static class GetAssortmentsEndpoint
{
    public static IEndpointRouteBuilder MapGetAssortments(this IEndpointRouteBuilder builder)
    {
        builder.MapGet(ApiEndpoints.Assortments.GetAssortments,
                async ([AsParameters] AssortmentsFilteringParams filteringParams, ISender mediatr,
                    CancellationToken cancellationToken) =>
                {
                    var query = new GetAssortmentsQuery { FilteringParams = filteringParams };
                    var result = await mediatr.Send(query, cancellationToken);

                    return result.Match(
                        Results.Ok,
                        ResultsHelper.Problem);
                })
            .Produces<FilteredResult<AssortmentResponse>>()
            .HasApiVersion(1.0)
            .WithName(nameof(ApiEndpoints.Assortments.GetAssortments));

        return builder;
    }
}
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Nexticz.Module.Vh.Application.Assortments.Queries.GetAssortmentById;
using Nexticz.Module.Vh.Contracts.Assortments;
using Nexticz.Lib.Shared.Errors.Models;
using Nexticz.Lib.Shared.Helpers;


namespace Nexticz.Module.Vh.Presentation.Endpoints.Assortments;

public static class GetAssortmentByIdEndpoint
{
    public static IEndpointRouteBuilder MapGetAssortmentById(this IEndpointRouteBuilder builder)
    {
        builder.MapGet(ApiEndpoints.Assortments.GetAssortmentById,
                async (Guid id, ISender mediatr, CancellationToken cancellationToken) =>
                {
                    var query = new GetAssortmentByIdQuery { Id = id };
                    var result = await mediatr.Send(query, cancellationToken);

                    return result.Match(
                        Results.Ok,
                        ResultsHelper.Problem);
                })
            .Produces<AssortmentResponse>()
            .Produces<ApiErrorResponse>(StatusCodes.Status404NotFound)
            .HasApiVersion(1.0)
            .WithName(nameof(ApiEndpoints.Assortments.GetAssortmentById));

        return builder;
    }
}
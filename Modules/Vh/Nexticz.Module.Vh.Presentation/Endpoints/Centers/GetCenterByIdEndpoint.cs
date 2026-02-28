using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Nexticz.Module.Vh.Application.Centers.Queries.GetCenterById;
using Nexticz.Module.Vh.Contracts.Centers;
using Nexticz.Lib.Shared.Errors.Models;
using Nexticz.Lib.Shared.Helpers;


namespace Nexticz.Module.Vh.Presentation.Endpoints.Centers;

public static class GetCenterByIdEndpoint
{
    public static IEndpointRouteBuilder MapGetCenterById(this IEndpointRouteBuilder builder)
    {
        builder.MapGet(ApiEndpoints.Centers.GetCenterById, async 
            (Guid id, ISender mediatr, CancellationToken cancellationToken) =>
        {
            var query = new GetCenterByIdQuery { Id = id };
            var result = await mediatr.Send(query, cancellationToken);

            return result.Match(
                Results.Ok,
                ResultsHelper.Problem);
        })
        .Produces<CenterResponse>()
        .Produces<ApiErrorResponse>(StatusCodes.Status404NotFound)
        .HasApiVersion(1.0)
        .WithName(nameof(ApiEndpoints.Centers.GetCenterById));
        
        return builder;
    }
}
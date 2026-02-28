using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Nexticz.Module.Vh.Application.NonDispensingActivities.Queries.GetNonDispensingActivityById;
using Nexticz.Module.Vh.Application.NonDispensingActivities.Queries.GetNonDispensingActivityBySlug;
using Nexticz.Module.Vh.Contracts.NonDispensingActivities;

namespace Nexticz.Module.Vh.Presentation.Endpoints.NonDispensingActivities;

public static class GetNonDispensingActivityByIdOrSlugEndpoint
{
    public static IEndpointRouteBuilder MapGetNonDispensingActivityByIdOrSlug(this IEndpointRouteBuilder builder)
    {
        builder.MapGet(ApiEndpoints.NonDispensingActivities.GetNonDispensingActivityByIdOrSlug,
                async (string idOrSlug, ISender mediatr, CancellationToken cancellationToken) =>
                {
                    var getNonDispendingActivityById = Guid.TryParse(idOrSlug, out var id);

                    var queryById = new GetNonDispensingActivityByIdQuery { Id = id };
                    var queryBySlug = new GetNonDispensingActivityBySlugQuery { Slug = idOrSlug };

                    var result = getNonDispendingActivityById
                        ? await mediatr.Send(queryById, cancellationToken)
                        : await mediatr.Send(queryBySlug, cancellationToken);

                    return result.Match(
                        Results.Ok,
                        _ => Results.NoContent());
                })
            .Produces<NonDispensingActivityResponse>()
            .Produces(StatusCodes.Status204NoContent)
            .HasApiVersion(1.0)
            .AllowAnonymous()
            .WithName(nameof(ApiEndpoints.NonDispensingActivities.GetNonDispensingActivityByIdOrSlug));

        return builder;
    }
}
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Nexticz.Module.Vh.Application.Partners.Queries.GetPartnerById;
using Nexticz.Module.Vh.Contracts.Partners;
using Nexticz.Lib.Shared.Errors.Models;
using Nexticz.Lib.Shared.Helpers;


namespace Nexticz.Module.Vh.Presentation.Endpoints.Partners;

public static class GetPartnerByIdEndpoint
{
    public static IEndpointRouteBuilder MapGetPartnerById(this IEndpointRouteBuilder builder)
    {
        builder.MapGet(ApiEndpoints.Partners.GetPartnerdById,
                async (Guid id, ISender mediatr, CancellationToken cancellationToken) =>
                {
                    var query = new GetPartnerByIdQuery { Id = id };
                    var result = await mediatr.Send(query, cancellationToken);

                    return result.Match(
                        Results.Ok,
                        ResultsHelper.Problem);
                })
            .Produces<PartnerResponse>()
            .Produces<ApiErrorResponse>(StatusCodes.Status404NotFound)
            .HasApiVersion(1.0)
            .WithName(nameof(ApiEndpoints.Partners.GetPartnerdById));

        return builder;
    }
}
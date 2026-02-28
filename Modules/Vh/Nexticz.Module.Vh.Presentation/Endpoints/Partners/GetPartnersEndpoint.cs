using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Nexticz.Module.Vh.Application.Partners.Common.Models;
using Nexticz.Module.Vh.Application.Partners.Queries.GetPartners;
using Nexticz.Module.Vh.Contracts.Partners;
using Nexticz.Lib.Shared.DevExtreme;
using Nexticz.Lib.Shared.Helpers;


namespace Nexticz.Module.Vh.Presentation.Endpoints.Partners;

public static class GetPartnersEndpoint
{
    public static IEndpointRouteBuilder MapGetPartners(this IEndpointRouteBuilder builder)
    {
        builder.MapGet(ApiEndpoints.Partners.GetPartners,
                async ([AsParameters] PartnersFilteringParams filteringParams, ISender mediatr,
                    CancellationToken cancellationToken) =>
                {
                    var query = new GetPartnersQuery { FilteringParams = filteringParams };
                    var result = await mediatr.Send(query, cancellationToken);

                    return result.Match(
                        Results.Ok,
                        ResultsHelper.Problem);
                })
            .Produces<FilteredResult<PartnerResponse>>()
            .HasApiVersion(1.0)
            .WithName(nameof(ApiEndpoints.Partners.GetPartners));

        return builder;
    }
}
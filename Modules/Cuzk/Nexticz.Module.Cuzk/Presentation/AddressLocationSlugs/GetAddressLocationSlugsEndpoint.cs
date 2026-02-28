using System.Web;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Nexticz.Module.Cuzk.Contracts.AddressLocationsSlugs;
using Nexticz.Module.Cuzk.Application.AddressLocations.Queries.GetAddressLocationsBySlug;

namespace Nexticz.Module.Cuzk.Presentation.AddressLocationSlugs;

internal static class GetAddressLocationSlugsEndpoint
{
    public static IEndpointRouteBuilder MapGetAddressLocationSlugsEndpoint(this IEndpointRouteBuilder builder)
    {
        builder.MapGet(CuzkEndpoints.AddressLocationSlugEndpoints.Get,
                async (
                    string searchQuery,
                    int? take,
                    ISender sender,
                    CancellationToken cancellationToken
                ) =>
                {
                    var decodedSlug = HttpUtility.UrlDecode(searchQuery);
                    var result =
                        await sender.Send(new GetAddressLocationsBySlugQuery(decodedSlug, take), cancellationToken);
                    
                    return Results.Ok(result.Select(AddressLocationSlugResponseFactory.Create));
                })
            .Produces<AddressLocationSlugResponse[]>()
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .HasApiVersion(1.0)
            .WithName(CuzkEndpoints.GetOpenApiName(nameof(CuzkEndpoints.AddressLocationSlugEndpoints.Get)));

        return builder;
    }
}
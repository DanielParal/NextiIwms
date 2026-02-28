using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Nexticz.Module.Cuzk.Contracts.AddressLocations;
using Nexticz.Lib.Shared.Helpers;
using Nexticz.Module.Cuzk.Application.AddressLocations.Queries.GetAddressLocationByAdmCode;

namespace Nexticz.Module.Cuzk.Presentation.AddressLocations;

internal static class GetAddressLocationByCodeEndpoint
{
    public static IEndpointRouteBuilder MapGetAddressLocationByCodeEndpoint(this IEndpointRouteBuilder builder)
    {
        builder.MapGet(CuzkEndpoints.AddressLocationEndpoints.GetAddressLocationByCode,
                async (
                    string code,
                    ISender sender,
                    CancellationToken cancellationToken
                ) =>
                {
                    var result =
                        await sender.Send(new GetAddressLocationByAdmCodeQuery(code), cancellationToken);

                    return result.Match(
                        addressLocation => Results.Ok(AddressLocationResponseFactory.Create(addressLocation)),
                        ResultsHelper.Problem);
                })
            .Produces<AddressLocationResponse>()
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status404NotFound)
            .HasApiVersion(1.0)
            .WithName(CuzkEndpoints.GetOpenApiName(nameof(CuzkEndpoints.AddressLocationEndpoints.GetAddressLocationByCode)));

        return builder;
    }
}
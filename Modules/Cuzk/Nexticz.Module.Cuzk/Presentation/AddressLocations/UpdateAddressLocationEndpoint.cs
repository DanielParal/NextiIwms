using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Nexticz.Module.Cuzk.Contracts.AddressLocations;
using Nexticz.Lib.Shared.Errors.Models;
using Nexticz.Lib.Shared.Helpers;
using Nexticz.Module.Cuzk.Application.AddressLocations.Commands.UpdateAddressLocation;

namespace Nexticz.Module.Cuzk.Presentation.AddressLocations;

internal static class UpdateAddressLocationEndpoint
{
    public static IEndpointRouteBuilder MapUpdateAddressLocationEndpoint(this IEndpointRouteBuilder builder)
    {
        builder.MapPut(CuzkEndpoints.AddressLocationEndpoints.UpdateAddressLocation,
                async (
                    string code,
                    UpdateAddressLocationRequest request,
                    ISender sender,
                    CancellationToken cancellationToken) =>
                {
                    var result =
                        await sender.Send(new UpdateAddressLocationCommand(code, request), cancellationToken);

                    return result.Match(
                        _ => Results.NoContent(),
                        ResultsHelper.Problem);
                })
            .Produces<object>(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces<ApiErrorResponse>(StatusCodes.Status422UnprocessableEntity)
            .HasApiVersion(1.0)
            .WithName(CuzkEndpoints.GetOpenApiName(nameof(CuzkEndpoints.AddressLocationEndpoints.UpdateAddressLocation)));

        return builder;
    }
}
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Nexticz.Lib.Shared.Errors.Models;
using Nexticz.Lib.Shared.Helpers;
using Nexticz.Module.Cuzk.Application.AddressLocations.Commands.DeleteAddressLocation;

namespace Nexticz.Module.Cuzk.Presentation.AddressLocations;

internal static class DeleteAddressLocationEndpoint
{
    public static IEndpointRouteBuilder MapDeleteAddressLocationEndpoint(this IEndpointRouteBuilder builder)
    {
        builder.MapDelete(CuzkEndpoints.AddressLocationEndpoints.DeleteAddressLocation,
                async (
                    string code,
                    ISender sender,
                    CancellationToken cancellationToken) =>
                {
                    var result =
                        await sender.Send(new DeleteAddressLocationCommand(code), cancellationToken);

                    return result.Match(
                        _ => Results.NoContent(),
                        ResultsHelper.Problem);
                })
            .Produces<object>(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces<ApiErrorResponse>(StatusCodes.Status422UnprocessableEntity)
            .HasApiVersion(1.0)
            .WithName(CuzkEndpoints.GetOpenApiName(nameof(CuzkEndpoints.AddressLocationEndpoints.DeleteAddressLocation)));

        return builder;
    }
}
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Nexticz.Module.Cuzk.Contracts.AddressLocations;
using Nexticz.Lib.Shared.Errors.Models;
using Nexticz.Lib.Shared.Helpers;
using Nexticz.Module.Cuzk.Application.AddressLocations.Commands.CreateAddressLocation;

namespace Nexticz.Module.Cuzk.Presentation.AddressLocations;

internal static class CreateAddressLocationEndpoint
{
    public static IEndpointRouteBuilder MapCreateAddressLocationEndpoint(this IEndpointRouteBuilder builder)
    {
        builder.MapPost(CuzkEndpoints.AddressLocationEndpoints.CreateAddressLocation,
                async (
                    CreateAddressLocationRequest request,
                    ISender sender,
                    CancellationToken cancellationToken) =>
                {
                    var result = 
                        await sender.Send(new CreateAddressLocationCommand(request), 
                            cancellationToken);

                    return result.Match(
                        addressLocation => Results.Created(
                            $"{CuzkEndpoints.AddressLocationEndpoints.GetAddressLocations}/{addressLocation.Id}",
                            AddressLocationResponseFactory.Create(addressLocation)),
                        ResultsHelper.Problem);
                })
            .Produces<AddressLocationResponse>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces<ApiErrorResponse>(StatusCodes.Status422UnprocessableEntity)
            .HasApiVersion(1.0)
            .WithName(CuzkEndpoints.GetOpenApiName(nameof(CuzkEndpoints.AddressLocationEndpoints.CreateAddressLocation)));

        return builder;
    }
}
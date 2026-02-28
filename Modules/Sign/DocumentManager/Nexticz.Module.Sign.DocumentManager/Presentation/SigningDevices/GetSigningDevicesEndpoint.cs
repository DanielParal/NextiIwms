using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Nexticz.Module.Sign.DocumentManager.Contracts.SigningDevices;
using Nexticz.Module.Sign.DocumentManager.Application.SigningDevices.Queries.GetSigningDeviceResponsesForUser;

namespace Nexticz.Module.Sign.DocumentManager.Presentation.SigningDevices;

internal static class GetSigningDevicesEndpoint
{
    public static IEndpointRouteBuilder MapGetSigningDevicesEndpoint(this IEndpointRouteBuilder builder)
    {
        builder.MapGet(DocumentManagerEndpoints.SigningDeviceEndpoints.GetSigningDevices,
                async (
                    ISender sender,
                    CancellationToken cancellationToken) =>
                {
                    var result = await sender.Send(new GetSigningDeviceResponsesForUserQuery(), cancellationToken);
                    
                    return Results.Ok(result);
                })
            .Produces<SigningDeviceResponse[]>()
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .HasApiVersion(1.0)
            .WithName(DocumentManagerEndpoints.GetOpenApiName(nameof(DocumentManagerEndpoints.SigningDeviceEndpoints.GetSigningDevices)));

        return builder;
    }
}
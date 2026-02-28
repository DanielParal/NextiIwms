using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Nexticz.Module.Sign.Settings.Application.SigningDevices;
using Nexticz.Module.Sign.Settings.Contracts.SigningDevices;
using Nexticz.Lib.Shared.Helpers;
using Nexticz.Module.Sign.Settings.Application.SigningDevices.Queries.GetSigningDeviceByCode;

namespace Nexticz.Module.Sign.Settings.Presentation.SigningDevices;

internal static class GetSigningDeviceByCodeEndpoint
{
    public static IEndpointRouteBuilder MapGetSigningDeviceByCodeEndpoint(this IEndpointRouteBuilder builder)
    {
        builder.MapGet(SettingsEndpoints.SigningDeviceEndpoints.GetSigningDeviceByCode,
                async (
                    string code, 
                    ISender sender,
                    CancellationToken cancellationToken
                ) =>
                {
                    var result = 
                        await sender.Send(new GetSigningDeviceByCodeQuery(code), cancellationToken);
                    
                    return result.Match(
                        signingDevice => Results.Ok(SigningDeviceResponseFactory.Create(signingDevice)),
                        ResultsHelper.Problem);
                })
            .Produces<SigningDeviceResponse>()
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status404NotFound)
            .HasApiVersion(1.0)
            .WithName(SettingsEndpoints.GetOpenApiName(nameof(SettingsEndpoints.SigningDeviceEndpoints.GetSigningDeviceByCode)));

        return builder;
    }
}
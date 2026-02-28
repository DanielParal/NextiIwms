using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Nexticz.Module.Mmo.Settings.Application.Kits;
using Nexticz.Module.Mmo.Settings.Application.Kits.Queries.GetKitByCode;
using Nexticz.Module.Mmo.Settings.Contracts.Kits;
using Nexticz.Lib.Shared.Helpers;
using Nexticz.Module.Mmo.Settings.Application.Kits.Queries.GetKitSpecialInformationResponsesByKitCode;

namespace Nexticz.Module.Mmo.Settings.Presentation.Kits;

internal static class GetKitSpecialInformationsEndpoint
{
    public static IEndpointRouteBuilder MapGetKitSpecialInformationsEndpoint(this IEndpointRouteBuilder builder)
    {
        builder.MapGet(SettingsEndpoints.KitEndpoints.GetKitSpecialInformations,
                async (
                    string code, 
                    ISender sender,
                    CancellationToken cancellationToken
                ) =>
                {
                    var result = await sender.Send(new GetKitSpecialInformationResponsesByKitCodeQuery(code), cancellationToken);
                    return Results.Ok(result);
                })
            .Produces<KitSpecialInformationResponse[]>()
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status404NotFound)
            .HasApiVersion(1.0)
            .WithName(SettingsEndpoints.GetOpenApiName(nameof(SettingsEndpoints.KitEndpoints.GetKitSpecialInformations)));

        return builder;
    }
}
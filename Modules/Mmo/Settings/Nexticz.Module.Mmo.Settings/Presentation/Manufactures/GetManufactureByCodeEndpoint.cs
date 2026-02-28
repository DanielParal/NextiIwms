using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Nexticz.Module.Mmo.Settings.Contracts.Manufactures;
using Nexticz.Lib.Shared.Helpers;
using Nexticz.Module.Mmo.Settings.Application.Manufactures.Queries.GetManufactureByCode;

namespace Nexticz.Module.Mmo.Settings.Presentation.Manufactures;

internal static class GetManufactureByCodeEndpoint
{
    public static IEndpointRouteBuilder MapGetManufactureByCode(this IEndpointRouteBuilder builder)
    {
        builder.MapGet(SettingsEndpoints.ManufactureEndpoints.GetManufactureByCode,
                async (
                    string code, 
                    ISender mediator,
                    CancellationToken cancellationToken
                ) =>
                {
                    var request = new GetManufactureByCodeQuery(code);
                    var result = await mediator.Send(request, cancellationToken);
                    return result.Match(
                        manufacture => Results.Ok(ManufactureResponseFactory.Create(manufacture)),
                        ResultsHelper.Problem);
                })
            .Produces<ManufactureResponse>()
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status404NotFound)
            .HasApiVersion(1.0)
            .WithName(SettingsEndpoints.GetOpenApiName(nameof(SettingsEndpoints.ManufactureEndpoints.GetManufactureByCode)));

        return builder;
    }
}
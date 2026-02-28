using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Nexticz.Module.Mmo.Settings.Contracts.Kits;
using Nexticz.Lib.Shared.Helpers;
using Nexticz.Module.Mmo.Settings.Application.Kits;
using Nexticz.Module.Mmo.Settings.Application.Kits.Queries.GetKitByCode;

namespace Nexticz.Module.Mmo.Settings.Presentation.Kits;

internal static class GetKitByCodeEndpoint
{
    public static IEndpointRouteBuilder MapGetKitById(this IEndpointRouteBuilder builder)
    {
        builder.MapGet(SettingsEndpoints.KitEndpoints.GetKitByCode,
                async (
                    string code, 
                    ISender mediator,
                    [FromServices] KitResponseFactory kitResponseFactory,
                    CancellationToken cancellationToken
                ) =>
                {
                    var request = new GetKitByCodeQuery(code);
                    var result = await mediator.Send(request, cancellationToken);
                    
                    if (result.IsError)
                    {
                        return ResultsHelper.Problem(result.Errors);
                    }
                    
                    var kitResponse = await kitResponseFactory.CreateAsync(result.Value, cancellationToken);
                    return Results.Ok(kitResponse);
                })
            .Produces<KitResponse>()
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status404NotFound)
            .HasApiVersion(1.0)
            .WithName(SettingsEndpoints.GetOpenApiName(nameof(SettingsEndpoints.KitEndpoints.GetKitByCode)));

        return builder;
    }
}
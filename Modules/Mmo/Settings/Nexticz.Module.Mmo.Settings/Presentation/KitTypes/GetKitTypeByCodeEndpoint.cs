using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Nexticz.Module.Mmo.Settings.Contracts.KitTypes;
using Nexticz.Lib.Shared.Helpers;
using Nexticz.Module.Mmo.Settings.Application.KitTypes.Queries.GetKitTypeByCode;

namespace Nexticz.Module.Mmo.Settings.Presentation.KitTypes;

internal static class GetKitTypeByCodeEndpoint
{
    public static IEndpointRouteBuilder MapGetKitTypeByCode(this IEndpointRouteBuilder builder)
    {
        builder.MapGet(SettingsEndpoints.KitTypeEndpoints.GetKitTypeByCode,
                async (
                    string code, 
                    ISender mediator,
                    CancellationToken cancellationToken
                    ) =>
                {
                    var query = new GetKitTypeByCodeQuery(code);
                    var result = await mediator.Send(query, cancellationToken);
                    return result.Match(
                        kitType => Results.Ok(KitTypeResponseFactory.Create(kitType)),
                        ResultsHelper.Problem);
                })
            .Produces<KitTypeResponse>()
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status404NotFound)
            .HasApiVersion(1.0)
            .WithName(SettingsEndpoints.GetOpenApiName(nameof(SettingsEndpoints.KitTypeEndpoints.GetKitTypeByCode)));

        return builder;
    }
}
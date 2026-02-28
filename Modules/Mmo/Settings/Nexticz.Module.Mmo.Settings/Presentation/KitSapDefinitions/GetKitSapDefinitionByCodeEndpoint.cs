using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Nexticz.Module.Mmo.Settings.Contracts.KitSapDefinitions;
using Nexticz.Lib.Shared.Helpers;
using Nexticz.Module.Mmo.Settings.Application.KitSapDefinitions;
using Nexticz.Module.Mmo.Settings.Application.KitSapDefinitions.Queries.GetKitSapDefinitionByCode;

namespace Nexticz.Module.Mmo.Settings.Presentation.KitSapDefinitions;

internal static class GetKitSapDefinitionByCodeEndpoint
{
    public static IEndpointRouteBuilder MapGetKitSapDefinitionByCode(this IEndpointRouteBuilder builder)
    {
        builder.MapGet(SettingsEndpoints.KitSapDefinitionEndpoints.GetKitSapDefinitionByCode,
                async (
                    string code, 
                    ISender mediator,
                    CancellationToken cancellationToken
                ) =>
                {
                    var query = new GetKitSapDefinitionByCodeQuery(code);
                    var result = await mediator.Send(query, cancellationToken);
                    return result.Match(
                        kitSapDefinition => Results.Ok(KitSapDefinitionResponseFactory.Create(kitSapDefinition)),
                        ResultsHelper.Problem);
                })
            .Produces<KitSapDefinitionResponse>()
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status404NotFound)
            .HasApiVersion(1.0)
            .WithName(SettingsEndpoints.GetOpenApiName(nameof(SettingsEndpoints.KitSapDefinitionEndpoints.GetKitSapDefinitionByCode)));

        return builder;
    }
}
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Nexticz.Module.Mmo.Settings.Contracts.KitSapDefinitions;
using Nexticz.Lib.Shared.Errors.Models;
using Nexticz.Lib.Shared.Helpers;
using Nexticz.Module.Mmo.Settings.Application.KitSapDefinitions;
using Nexticz.Module.Mmo.Settings.Application.KitSapDefinitions.Commands.CreateKitSapDefinition;


namespace Nexticz.Module.Mmo.Settings.Presentation.KitSapDefinitions;

internal static class CreateKitSapDefinitionEndpoint
{
    public static IEndpointRouteBuilder MapCreateKitSapDefinition(this IEndpointRouteBuilder builder)
    {
        builder.MapPost(SettingsEndpoints.KitSapDefinitionEndpoints.CreateKitSapDefinition,
                async (
                    CreateKitSapDefinitionRequest request,
                    ISender sender,
                    CancellationToken cancellationToken) =>
                {
                    var result = 
                        await sender.Send(
                            new CreateKitSapDefinitionCommand(request.Code, request.Name), 
                            cancellationToken);

                    return result.Match(
                        kitSapDefinition => Results.Created(
                            $"{SettingsEndpoints.KitSapDefinitionEndpoints.GetKitSapDefinitions}/{kitSapDefinition.Code}",
                            KitSapDefinitionResponseFactory.Create(kitSapDefinition)),
                        ResultsHelper.Problem);
                })
            .Produces<KitSapDefinitionResponse>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces<ApiErrorResponse>(StatusCodes.Status422UnprocessableEntity)
            .HasApiVersion(1.0)
            .WithName(SettingsEndpoints.GetOpenApiName(nameof(SettingsEndpoints.KitSapDefinitionEndpoints.CreateKitSapDefinition)));

        return builder;
    }
}
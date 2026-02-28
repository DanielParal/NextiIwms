using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Nexticz.Module.Mmo.Settings.Contracts.SpecialInformations;
using Nexticz.Lib.Shared.Errors.Models;
using Nexticz.Lib.Shared.Helpers;
using Nexticz.Module.Mmo.Settings.Application.SpecialInformations;
using Nexticz.Module.Mmo.Settings.Application.SpecialInformations.Commands.CreateSpecialInformation;


namespace Nexticz.Module.Mmo.Settings.Presentation.SpecialInformations;

internal static class CreateSpecialInformationEndpoint
{
    public static IEndpointRouteBuilder MapCreateSpecialInformationEndpoint(this IEndpointRouteBuilder builder)
    {
        builder.MapPost(SettingsEndpoints.SpecialInformationEndpoints.CreateSpecialInformation,
                async (
                    CreateSpecialInformationRequest request, 
                    ISender sender, 
                    CancellationToken cancellationToken) =>
                {
                    var result = await sender.Send(new CreateSpecialInformationCommand(request.Title, request.Description), cancellationToken);
                    
                    return result.Match(
                        specialInformation => Results.Created($"{SettingsEndpoints.SpecialInformationEndpoints.GetSpecialInformations}/{specialInformation.Id}",
                            SpecialInformationResponseFactory.Create(specialInformation)),
                        ResultsHelper.Problem);
                })
            .Produces<SpecialInformationResponse>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces<ApiErrorResponse>(StatusCodes.Status422UnprocessableEntity)
            .HasApiVersion(1.0)
            .WithName(SettingsEndpoints.GetOpenApiName(nameof(SettingsEndpoints.SpecialInformationEndpoints.CreateSpecialInformation)));

        return builder;
    }
}
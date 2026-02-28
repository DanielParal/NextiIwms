using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Nexticz.Module.Mmo.Settings.Contracts.KitTypes;
using Nexticz.Lib.Shared.Errors.Models;
using Nexticz.Lib.Shared.Helpers;
using Nexticz.Module.Mmo.Settings.Application.KitTypes.Commands.CreateKitType;


namespace Nexticz.Module.Mmo.Settings.Presentation.KitTypes;

internal static class CreateKitTypeEndpoint
{
    public static IEndpointRouteBuilder MapCreateKitType(this IEndpointRouteBuilder builder)
    {
        builder.MapPost(SettingsEndpoints.KitTypeEndpoints.CreateKitType,
                async (
                    CreateKitTypeRequest request,
                    ISender mediator,
                    CancellationToken cancellationToken) =>
                {
                    var createKitTypeCommand = new CreateKitTypeCommand(request.Code, request.Name);
                    var result = await mediator.Send(createKitTypeCommand, cancellationToken);

                    return result.Match(
                        kitType => Results.Created(
                            $"{SettingsEndpoints.KitTypeEndpoints.GetKitTypes}/{kitType.Code}",
                            KitTypeResponseFactory.Create(kitType)),
                        ResultsHelper.Problem);
                })
            .Produces<KitTypeResponse>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces<ApiErrorResponse>(StatusCodes.Status422UnprocessableEntity)
            .HasApiVersion(1.0)
            .WithName(SettingsEndpoints.GetOpenApiName(nameof(SettingsEndpoints.KitTypeEndpoints.CreateKitType)));

        return builder;
    }
}
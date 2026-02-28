using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Nexticz.Module.Mmo.Settings.Contracts.InactivityTypes;
using Nexticz.Lib.Shared.Errors.Models;
using Nexticz.Lib.Shared.Helpers;
using Nexticz.Module.Mmo.Settings.Application.InactivityTypes.Commands.UpdateInactivityType;


namespace Nexticz.Module.Mmo.Settings.Presentation.InactivityTypes;

internal static class UpdateInactivityTypeEndpoint
{
    public static IEndpointRouteBuilder MapUpdateInactivityTypeEndpoint(this IEndpointRouteBuilder builder)
    {
        builder.MapPut(SettingsEndpoints.InactivityTypeEndpoints.UpdateInactivityType,
                async (
                    Guid id,
                    UpdateInactivityTypeRequest request, 
                    ISender sender, 
                    CancellationToken cancellationToken) =>
                {
                    var result = await sender.Send(new UpdateInactivityTypeCommand(id, request.Name, request.AffectProductivity, request.IsCommentNeededForReview), cancellationToken);

                    return result.Match(
                        _ => Results.NoContent(),
                        ResultsHelper.Problem);
                })
            .Produces<object>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces<ApiErrorResponse>(StatusCodes.Status404NotFound)
            .Produces<ApiErrorResponse>(StatusCodes.Status422UnprocessableEntity)
            .HasApiVersion(1.0)
            .WithName(SettingsEndpoints.GetOpenApiName(nameof(SettingsEndpoints.InactivityTypeEndpoints.UpdateInactivityType)));

        return builder;
    }
}
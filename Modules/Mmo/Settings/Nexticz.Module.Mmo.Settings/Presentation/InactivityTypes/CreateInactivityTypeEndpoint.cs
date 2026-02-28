using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Nexticz.Module.Mmo.Settings.Contracts.InactivityTypes;
using Nexticz.Lib.Shared.Errors.Models;
using Nexticz.Lib.Shared.Helpers;
using Nexticz.Module.Mmo.Settings.Application.InactivityTypes;
using Nexticz.Module.Mmo.Settings.Application.InactivityTypes.Commands.CreateInactivityType;


namespace Nexticz.Module.Mmo.Settings.Presentation.InactivityTypes;

internal static class CreateInactivityTypeEndpoint
{
    public static IEndpointRouteBuilder MapCreateInactivityTypeEndpoint(this IEndpointRouteBuilder builder)
    {
        builder.MapPost(SettingsEndpoints.InactivityTypeEndpoints.CreateInactivityType,
                async (
                    CreateInactivityTypeRequest request,
                    ISender sender,
                    CancellationToken cancellationToken) =>
                {
                    var result = await sender.Send(
                        new CreateInactivityTypeCommand(request.Name, request.AffectProductivity, request.IsCommentNeededForReview), 
                        cancellationToken);
        
                    return result.Match(
                        inactivityType => 
                            Results.Created($"/{SettingsEndpoints.InactivityTypeEndpoints.CreateInactivityType}/{inactivityType.Id}", 
                                InactivityTypeResponseFactory.Create(inactivityType)),
                        ResultsHelper.Problem);
                })
            .Produces<InactivityTypeResponse>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces<ApiErrorResponse>(StatusCodes.Status422UnprocessableEntity)
            .HasApiVersion(1.0)
            .WithName(SettingsEndpoints.GetOpenApiName(nameof(SettingsEndpoints.InactivityTypeEndpoints.CreateInactivityType)));

        return builder;
    }
}
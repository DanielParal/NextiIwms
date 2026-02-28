using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Nexticz.Module.Mmo.Settings.Contracts.Users;
using Nexticz.Lib.Shared.Errors.Models;
using Nexticz.Lib.Shared.Helpers;
using Nexticz.Module.Mmo.Settings.Application.Users.Commands.UpdateUser;
using Nexticz.Module.Mmo.Settings.Domain.UserEntity;


namespace Nexticz.Module.Mmo.Settings.Presentation.Users;

internal static class UpdateUserEndpoint
{
    public static IEndpointRouteBuilder MapUpdateUserEndpoint(this IEndpointRouteBuilder builder)
    {
        builder.MapPut(SettingsEndpoints.UserEndpoints.UpdateUser,
                async (
                    Guid id, 
                    UpdateUserRequest request, 
                    ISender sender,
                    CancellationToken cancellationToken) =>
                {
                    var result = await sender.Send(
                        new UpdateUserCommand(id, request.ReceivableNotifications.Select(x => (ReceivableNotification)x).ToArray()), 
                        cancellationToken);

                    return result.Match(
                        _ => Results.NoContent(),
                        ResultsHelper.Problem);
                })
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces<ApiErrorResponse>(StatusCodes.Status404NotFound)
            .Produces<ApiErrorResponse>(StatusCodes.Status422UnprocessableEntity)
            .HasApiVersion(1.0)
            .WithName(SettingsEndpoints.GetOpenApiName(nameof(SettingsEndpoints.UserEndpoints.UpdateUser)));

        return builder;
    }
}
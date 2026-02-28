using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Nexticz.Lib.Shared.Errors.Models;
using Nexticz.Lib.Shared.Helpers;
using Nexticz.Module.Sign.Settings.Application.Users.Commands.DeleteUserSignature;

namespace Nexticz.Module.Sign.Settings.Presentation.Users;

internal static class DeleteUserSignatureEndpoint
{
    public static IEndpointRouteBuilder MapDeleteUserSignatureEndpoint(this IEndpointRouteBuilder builder)
    {
        builder.MapDelete(SettingsEndpoints.UserEndpoints.DeleteSignature,
                async (
                    string userName,
                    ISender sender,
                    CancellationToken cancellationToken) =>
                {
                    var result = 
                        await sender.Send(new DeleteUserSignatureCommand(userName), cancellationToken);
                    
                    return result.Match(
                        _ => Results.NoContent(),
                        ResultsHelper.Problem);
                })
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces<ApiErrorResponse>(StatusCodes.Status422UnprocessableEntity)
            .HasApiVersion(1.0)
            .WithName(SettingsEndpoints.GetOpenApiName(nameof(SettingsEndpoints.UserEndpoints.DeleteSignature)));

        return builder;
    }
}
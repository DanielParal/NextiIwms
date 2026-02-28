using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Nexticz.Module.Mmo.Settings.Contracts.Packagings;
using Nexticz.Lib.Shared.Errors.Models;
using Nexticz.Lib.Shared.Helpers;
using Nexticz.Module.Mmo.Settings.Application.Packagings.Commands.UpdatePackaging;


namespace Nexticz.Module.Mmo.Settings.Presentation.Packagings;

internal static class UpdatePackagingEndpoint
{
    public static IEndpointRouteBuilder MapUpdatePackaging(this IEndpointRouteBuilder builder)
    {
        builder.MapPut(SettingsEndpoints.PackagingEndpoints.UpdatePackaging,
                async (
                    string code, 
                    UpdatePackagingRequest request, 
                    ISender mediatr,
                    CancellationToken cancellationToken) =>
                {
                    var command = new UpdatePackagingCommand(code, request);
                    var result = await mediatr.Send(command, cancellationToken);

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
            .WithName(SettingsEndpoints.GetOpenApiName(nameof(SettingsEndpoints.PackagingEndpoints.UpdatePackaging)));

        return builder;
    }
}
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Nexticz.Module.Mmo.Settings.Contracts.PackagingTypes;
using Nexticz.Lib.Shared.Errors.Models;
using Nexticz.Lib.Shared.Helpers;
using Nexticz.Module.Mmo.Settings.Application.PackagingTypes.Commands.UpdatePackagingType;


namespace Nexticz.Module.Mmo.Settings.Presentation.PackagingTypes;

internal static class UpdatePackagingTypeEndpoint
{
    public static IEndpointRouteBuilder MapUpdatePackagingType(this IEndpointRouteBuilder builder)
    {
        builder.MapPut(SettingsEndpoints.PackagingTypeEndpoints.UpdatePackagingType,
                async (
                    string code, 
                    UpdatePackagingTypeRequest request, 
                    ISender mediatr,
                    CancellationToken cancellationToken) =>
                {
                    var command = new UpdatePackagingTypeCommand(code, request.Name);
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
            .WithName(SettingsEndpoints.GetOpenApiName(nameof(SettingsEndpoints.PackagingTypeEndpoints.UpdatePackagingType)));

        return builder;
    }
}
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Nexticz.Module.Mmo.Settings.Contracts.PackagingTypes;
using Nexticz.Lib.Shared.Errors.Models;
using Nexticz.Lib.Shared.Helpers;
using Nexticz.Module.Mmo.Settings.Application.PackagingTypes.Commands.CreatePackagingType;


namespace Nexticz.Module.Mmo.Settings.Presentation.PackagingTypes;

internal static class CreatePackagingTypeEndpoint
{
    public static IEndpointRouteBuilder MapCreatePackagingType(this IEndpointRouteBuilder builder)
    {
        builder.MapPost(SettingsEndpoints.PackagingTypeEndpoints.CreatePackagingType,
                async (
                    CreatePackagingTypeRequest request, 
                    ISender mediator, 
                    CancellationToken cancellationToken) =>
                {
                    var createPackageType = new CreatePackagingTypeCommand(request.Code, request.Name);
                    var result = await mediator.Send(createPackageType, cancellationToken);

                    return result.Match(
                        packagingType => 
                            Results.Created($"{SettingsEndpoints.PackagingTypeEndpoints.GetPackagingTypes}/{packagingType.Id}", 
                                PackagingTypeResponseFactory.Create(packagingType)),
                        ResultsHelper.Problem);
                })
            .Produces<PackagingTypeResponse>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces<ApiErrorResponse>(StatusCodes.Status422UnprocessableEntity)
            .HasApiVersion(1.0)
            .WithName(SettingsEndpoints.GetOpenApiName(nameof(SettingsEndpoints.PackagingTypeEndpoints.CreatePackagingType)));

        return builder;
    }
}
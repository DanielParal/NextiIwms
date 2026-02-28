using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Nexticz.Module.Mmo.Settings.Application.PackagingTypes.Queries.GetPackagingTypeByCode;
using Nexticz.Module.Mmo.Settings.Contracts.Packagings;
using Nexticz.Lib.Shared.Errors.Models;
using Nexticz.Lib.Shared.Helpers;
using Nexticz.Module.Mmo.Settings.Application.Packagings;
using Nexticz.Module.Mmo.Settings.Application.Packagings.Commands.CreatePackaging;


namespace Nexticz.Module.Mmo.Settings.Presentation.Packagings;

internal static class CreatePackagingEndpoint
{
    public static IEndpointRouteBuilder MapCreatePackaging(this IEndpointRouteBuilder builder)
    {
        builder.MapPost(SettingsEndpoints.PackagingEndpoints.CreatePackaging,
                async (
                    CreatePackagingRequest request, 
                    ISender sender, 
                    CancellationToken cancellationToken) =>
                {
                    var result = await sender.Send(new CreatePackagingCommand(request), cancellationToken);
                    
                    return result.Match(
                        packaging => Results.Created($"{SettingsEndpoints.PackagingEndpoints.GetPackagings}/{packaging.Id}", 
                            PackagingResponseFactory.Create(packaging)),
                        ResultsHelper.Problem);
                })
            .Produces<PackagingResponse>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces<ApiErrorResponse>(StatusCodes.Status422UnprocessableEntity)
            .HasApiVersion(1.0)
            .WithName(SettingsEndpoints.GetOpenApiName(nameof(SettingsEndpoints.PackagingEndpoints.CreatePackaging)));

        return builder;
    }
}
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Nexticz.Module.Mmo.Settings.Application.PackagingTypes.Queries.GetPackagingTypeByCode;
using Nexticz.Module.Mmo.Settings.Contracts.Packagings;
using Nexticz.Lib.Shared.Helpers;
using Nexticz.Module.Mmo.Settings.Application.Packagings;
using Nexticz.Module.Mmo.Settings.Application.Packagings.Queries.GetPackagingByCode;

namespace Nexticz.Module.Mmo.Settings.Presentation.Packagings;

internal static class GetPackagingByIdEndpoint
{
    public static IEndpointRouteBuilder MapGetPackagingById(this IEndpointRouteBuilder builder)
    {
        builder.MapGet(SettingsEndpoints.PackagingEndpoints.GetPackagingByCode,
                async (
                    string code, 
                    ISender sender,
                    CancellationToken cancellationToken
                ) =>
                {
                    var result = await sender.Send(new GetPackagingByCodeQuery(code), cancellationToken);
                    
                    return result.Match(
                        packaging => Results.Ok(PackagingResponseFactory.Create(packaging)),
                        ResultsHelper.Problem);
                })
            .Produces<PackagingResponse>()
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status404NotFound)
            .HasApiVersion(1.0)
            .WithName(SettingsEndpoints.GetOpenApiName(nameof(SettingsEndpoints.PackagingEndpoints.GetPackagingByCode)));

        return builder;
    }
}
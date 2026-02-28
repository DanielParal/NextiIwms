using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Nexticz.Module.Mmo.Settings.Contracts.PackagingTypes;
using Nexticz.Lib.Shared.Helpers;
using Nexticz.Module.Mmo.Settings.Application.PackagingTypes.Queries.GetPackagingTypeByCode;

namespace Nexticz.Module.Mmo.Settings.Presentation.PackagingTypes;

internal static class GetPackagingTypeByCodeEndpoint
{
    public static IEndpointRouteBuilder MapGetPackagingTypeByCode(this IEndpointRouteBuilder builder)
    {
        builder.MapGet(SettingsEndpoints.PackagingTypeEndpoints.GetPackagingTypeByCode,
                async (
                    string code, 
                    ISender mediator,
                    CancellationToken cancellationToken
                ) =>
                {
                    var query = new GetPackagingTypeByCodeQuery(code);
                    var result = await mediator.Send(query, cancellationToken);
                    return result.Match(
                        packagingType => Results.Ok(PackagingTypeResponseFactory.Create(packagingType)),
                        ResultsHelper.Problem);
                })
            .Produces<PackagingTypeResponse>()
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status404NotFound)
            .HasApiVersion(1.0)
            .WithName(SettingsEndpoints.GetOpenApiName(nameof(SettingsEndpoints.PackagingTypeEndpoints.GetPackagingTypeByCode)));

        return builder;
    }
}
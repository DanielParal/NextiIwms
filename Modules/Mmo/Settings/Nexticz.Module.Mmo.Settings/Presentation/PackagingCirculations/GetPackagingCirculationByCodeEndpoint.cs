using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Nexticz.Module.Mmo.Settings.Contracts.PackagingCirculations;
using Nexticz.Lib.Shared.Helpers;
using Nexticz.Module.Mmo.Settings.Application.PackagingCirculations.Queries.GetPackagingCirculationByCode;

namespace Nexticz.Module.Mmo.Settings.Presentation.PackagingCirculations;

internal static class GetPackagingCirculationByCodeEndpoint
{
    public static IEndpointRouteBuilder MapGetPackagingCirculationByCode(this IEndpointRouteBuilder builder)
    {
        builder.MapGet(SettingsEndpoints.PackagingCirculationEndpoints.GetPackagingCirculationByCode,
                async (
                    string code, 
                    ISender mediator,
                    CancellationToken cancellationToken
                ) =>
                {
                    var request = new GetPackagingCirculationByCodeQuery(code);
                    var result = await mediator.Send(request, cancellationToken);
                    return result.Match(
                        packagingCirculation => Results.Ok(PackagingCirculationResponseFactory.Create(packagingCirculation)),
                        ResultsHelper.Problem);
                })
            .Produces<PackagingCirculationResponse>()
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status404NotFound)
            .HasApiVersion(1.0)
            .WithName(SettingsEndpoints.GetOpenApiName(nameof(SettingsEndpoints.PackagingCirculationEndpoints.GetPackagingCirculationByCode)));

        return builder;
    }
}
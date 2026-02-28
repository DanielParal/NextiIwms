using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Nexticz.Module.Mmo.Settings.Contracts.SpecialInformations;
using Nexticz.Lib.Shared.Helpers;
using Nexticz.Module.Mmo.Settings.Application.SpecialInformations;
using Nexticz.Module.Mmo.Settings.Application.SpecialInformations.Queries.GetSpecialInformationById;

namespace Nexticz.Module.Mmo.Settings.Presentation.SpecialInformations;

internal static class GetSpecialInformationByIdEndpoint
{
    public static IEndpointRouteBuilder MapGetSpecialInformationByIdEndpoint(this IEndpointRouteBuilder builder)
    {
        builder.MapGet(SettingsEndpoints.SpecialInformationEndpoints.GetSpecialInformationById,
                async (
                    Guid id, 
                    ISender sender,
                    CancellationToken cancellationToken
                ) =>
                {
                    var result = await sender.Send(new GetSpecialInformationByIdQuery(id), cancellationToken);
                    
                    return result.Match(
                        specialInformation => Results.Ok(SpecialInformationResponseFactory.Create(specialInformation)),
                        ResultsHelper.Problem);
                })
            .Produces<SpecialInformationResponse>()
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status404NotFound)
            .HasApiVersion(1.0)
            .WithName(SettingsEndpoints.GetOpenApiName(nameof(SettingsEndpoints.SpecialInformationEndpoints.GetSpecialInformationById)));

        return builder;
    }
}
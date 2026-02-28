using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Nexticz.Module.Mmo.Settings.Contracts.InactivityTypes;
using Nexticz.Lib.Shared.Helpers;
using Nexticz.Module.Mmo.Settings.Application.InactivityTypes;
using Nexticz.Module.Mmo.Settings.Application.InactivityTypes.Queries.GetInactivityTypeById;

namespace Nexticz.Module.Mmo.Settings.Presentation.InactivityTypes;

internal static class GetInactivityTypeByIdEndpoint
{
    public static IEndpointRouteBuilder MapGetInactivityTypeByIdEndpoint(this IEndpointRouteBuilder builder)
    {
        builder.MapGet(SettingsEndpoints.InactivityTypeEndpoints.GetInactivityTypeById,
                async (
                    Guid id, 
                    ISender sender,
                    CancellationToken cancellationToken
                ) =>
                {
                    var request = new GetInactivityTypeByIdQuery(id);
                    var result = await sender.Send(request, cancellationToken);
                    return result.Match(
                        inactivityType => Results.Ok(InactivityTypeResponseFactory.Create(inactivityType)),
                        ResultsHelper.Problem);
                })
            .Produces<InactivityTypeResponse>()
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status404NotFound)
            .HasApiVersion(1.0)
            .WithName(SettingsEndpoints.GetOpenApiName(nameof(SettingsEndpoints.InactivityTypeEndpoints.GetInactivityTypeById)));

        return builder;
    }
}
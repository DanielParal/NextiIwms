using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Nexticz.Module.Sign.Settings.Contracts.DepositorGroups;
using Nexticz.Lib.Shared.Helpers;
using Nexticz.Module.Sign.Settings.Application.DepositorGroups.Queries.GetDepositorGroupByCode;

namespace Nexticz.Module.Sign.Settings.Presentation.DepositorGroups;

internal static class GetDepositorGroupByCodeEndpoint
{
    public static IEndpointRouteBuilder MapGetDepositorGroupByCodeEndpoint(this IEndpointRouteBuilder builder)
    {
        builder.MapGet(SettingsEndpoints.DepositorGroupEndpoints.GetDepositorGroupByCode,
                async (
                    string code, 
                    ISender sender,
                    CancellationToken cancellationToken
                ) =>
                {
                    var result = 
                        await sender.Send(new GetDepositorGroupByCodeQuery(code), cancellationToken);
                    
                    return result.Match(
                        depositor => Results.Ok(DepositorGroupResponseFactory.Create(depositor)),
                        ResultsHelper.Problem);
                })
            .Produces<DepositorGroupResponse>()
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status404NotFound)
            .HasApiVersion(1.0)
            .WithName(SettingsEndpoints.GetOpenApiName(nameof(SettingsEndpoints.DepositorGroupEndpoints.GetDepositorGroupByCode)));

        return builder;
    }
}
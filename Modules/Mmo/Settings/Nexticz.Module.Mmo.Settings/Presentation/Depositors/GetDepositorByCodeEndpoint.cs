using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Nexticz.Module.Mmo.Settings.Contracts.Depositors;
using Nexticz.Lib.Shared.Helpers;
using Nexticz.Module.Mmo.Settings.Application.Depositors;
using Nexticz.Module.Mmo.Settings.Application.Depositors.Queries.GetDepositorByCode;

namespace Nexticz.Module.Mmo.Settings.Presentation.Depositors;

internal static class GetDepositorByCodeEndpoint
{
    public static IEndpointRouteBuilder MapGetDepositorByCode(this IEndpointRouteBuilder builder)
    {
        builder.MapGet(SettingsEndpoints.DepositorEndpoints.GetDepositorByCode,
                async (
                    string code, 
                    ISender mediator,
                    CancellationToken cancellationToken
                    ) =>
                {
                    var query = new GetDepositorByCodeQuery(code);
                    var result = await mediator.Send(query, cancellationToken);
                    return result.Match(
                        depositor => Results.Ok(DepositorResponseFactory.Create(depositor)),
                        ResultsHelper.Problem);
                })
            .Produces<DepositorResponse>()
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status404NotFound)
            .HasApiVersion(1.0)
            .WithName(SettingsEndpoints.GetOpenApiName(nameof(SettingsEndpoints.DepositorEndpoints.GetDepositorByCode)));

        return builder;
    }
}
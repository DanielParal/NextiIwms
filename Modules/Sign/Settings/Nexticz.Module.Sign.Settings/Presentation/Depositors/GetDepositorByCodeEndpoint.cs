using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Nexticz.Module.Sign.Settings.Contracts.Depositors;
using Nexticz.Lib.Shared.Helpers;
using Nexticz.Module.Sign.Settings.Application.Depositors;
using Nexticz.Module.Sign.Settings.Application.Depositors.Queries.GetDepositorByCode;

namespace Nexticz.Module.Sign.Settings.Presentation.Depositors;

internal static class GetDepositorByCodeEndpoint
{
    public static IEndpointRouteBuilder MapGetDepositorByCodeEndpoint(this IEndpointRouteBuilder builder)
    {
        builder.MapGet(SettingsEndpoints.DepositorEndpoints.GetDepositorByCode,
                async (
                    string code, 
                    ISender sender,
                    CancellationToken cancellationToken
                ) =>
                {
                    var result = 
                        await sender.Send(new GetDepositorByCodeQuery(code), cancellationToken);
                    
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
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Nexticz.Module.Mmo.Settings.Contracts.WashingMachines;
using Nexticz.Lib.Shared.Helpers;
using Nexticz.Module.Mmo.Settings.Application.WashingMachines;
using Nexticz.Module.Mmo.Settings.Application.WashingMachines.Queries.GetWashingMachineByCode;

namespace Nexticz.Module.Mmo.Settings.Presentation.WashingMachines;

internal static class GetWashingMachineByCodeEndpoint
{
    public static IEndpointRouteBuilder MapGetWashingMachineByCode(this IEndpointRouteBuilder builder)
    {
        builder.MapGet(SettingsEndpoints.WashingMachineEndpoints.GetWashingMachineByCode,
                async (
                    string code, 
                    ISender mediator,
                    CancellationToken cancellationToken
                ) =>
                {
                    var query = new GetWashingMachineByCodeQuery(code);
                    var result = await mediator.Send(query, cancellationToken);
                    return result.Match(
                        washingMachine => Results.Ok(WashingMachineResponseFactory.Create(washingMachine)),
                        ResultsHelper.Problem);
                })
            .Produces<WashingMachineResponse>()
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status404NotFound)
            .HasApiVersion(1.0)
            .WithName(SettingsEndpoints.GetOpenApiName(nameof(SettingsEndpoints.WashingMachineEndpoints.GetWashingMachineByCode)));

        return builder;
    }
}
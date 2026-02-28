using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Nexticz.Module.Mmo.Planning.Contracts.WashingMachines;
using Nexticz.Module.Mmo.Washing.Contracts.WashingMachineSoses.Queries;
using Nexticz.Module.Mmo.Planning.Application.WashingMachines;
using Nexticz.Module.Mmo.Planning.Application.WashingMachineTimeTables.Queries.GetSuitableWashingMachinesTimeTablesByPackagingCode;
using Nexticz.Module.Mmo.Planning.Application.WashingMachineTimeTables.Queries.GetWashingMachinesTimeTables;

namespace Nexticz.Module.Mmo.Planning.Presentation.WashingMachines;

internal static class GetWashingMachinesEndpoint
{
    public static IEndpointRouteBuilder MapGetWashingMachinesEndpoint(this IEndpointRouteBuilder builder)
    {
        builder.MapGet(PlanningEndpoints.WashingMachineEndpoints.GetWashingMachines,
                async (
                    [AsParameters] GetWashingMachinesFilteringParams suitableWashingMachinesParams,
                    ISender sender,
                    CancellationToken cancellationToken
                ) =>
                {
                    if (suitableWashingMachinesParams.PackagingCode is not null)
                    {
                        var suitableWashingMachinesTimeTables = 
                            await sender.Send(
                                new GetSuitableWashingMachinesTimeTablesByPackagingCodeQuery(
                                    suitableWashingMachinesParams.PackagingCode, suitableWashingMachinesParams.SisterPackagingCode), cancellationToken);
                        var soses = await sender.Send(new GetWashingMachineSosResponsesQuery(), cancellationToken);
                        return Results.Ok(suitableWashingMachinesTimeTables.Select(x => WashingMachineResponseFactory.Create(x, soses)));
                    }
                    
                    var washingMachinesTimeTables = 
                        await sender.Send(new GetWashingMachinesTimeTablesQuery(), cancellationToken);
                    var sosResponses = await sender.Send(new GetWashingMachineSosResponsesQuery(), cancellationToken);
                    return Results.Ok(washingMachinesTimeTables.Select(x => WashingMachineResponseFactory.Create(x, sosResponses)));
                })
            .Produces<WashingMachineResponse[]>()
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .HasApiVersion(1.0)
            .WithName(PlanningEndpoints.GetOpenApiName(nameof(PlanningEndpoints.WashingMachineEndpoints.GetWashingMachines)));

        return builder;
    }
}
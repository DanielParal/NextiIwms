using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Nexticz.Module.Mmo.Settings.Contracts.WashingMachines;
using Nexticz.Lib.Shared.Errors.Models;
using Nexticz.Lib.Shared.Helpers;
using Nexticz.Module.Mmo.Settings.Application.WashingMachines;
using Nexticz.Module.Mmo.Settings.Application.WashingMachines.Commands.CreateWashingMachine;


namespace Nexticz.Module.Mmo.Settings.Presentation.WashingMachines;

internal static class CreateWashingMachineEndpoint
{
    public static IEndpointRouteBuilder MapCreateWashingMachine(this IEndpointRouteBuilder builder)
    {
        builder.MapPost(SettingsEndpoints.WashingMachineEndpoints.CreateWashingMachine,
                async (
                    CreateWashingMachineRequest request, 
                    ISender mediator, 
                    CancellationToken cancellationToken) =>
                {
                    var createCommand = new CreateWashingMachineCommand(request);
                    var result = await mediator.Send(createCommand, cancellationToken);

                    return result.Match(
                        washingMachine => 
                            Results.Created($"{SettingsEndpoints.WashingMachineEndpoints.GetWashingMachines}/{washingMachine.Code}", 
                                WashingMachineResponseFactory.Create(washingMachine)),
                        ResultsHelper.Problem);
                })
            .Produces<WashingMachineResponse>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces<ApiErrorResponse>(StatusCodes.Status422UnprocessableEntity)
            .HasApiVersion(1.0)
            .WithName(SettingsEndpoints.GetOpenApiName(nameof(SettingsEndpoints.WashingMachineEndpoints.CreateWashingMachine)));

        return builder;
    }
}
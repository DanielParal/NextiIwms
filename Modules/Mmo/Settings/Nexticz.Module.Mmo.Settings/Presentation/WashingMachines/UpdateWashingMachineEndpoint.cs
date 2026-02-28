using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Nexticz.Module.Mmo.Settings.Contracts.WashingMachines;
using Nexticz.Lib.Shared.Errors.Models;
using Nexticz.Lib.Shared.Helpers;
using Nexticz.Module.Mmo.Settings.Application.WashingMachines.Commands.UpdateWashingMachine;


namespace Nexticz.Module.Mmo.Settings.Presentation.WashingMachines;

internal static class UpdateWashingMachineEndpoint
{
    public static IEndpointRouteBuilder MapUpdateWashingMachine(this IEndpointRouteBuilder builder)
    {
        builder.MapPut(SettingsEndpoints.WashingMachineEndpoints.UpdateWashingMachine,
                async (
                    string code, 
                    UpdateWashingMachineRequest request, 
                    ISender mediatr,
                    CancellationToken cancellationToken) =>
                {
                    var command = new UpdateWashingMachineCommand(code, request);
                    var result = await mediatr.Send(command, cancellationToken);

                    return result.Match(
                        _ => Results.NoContent(),
                        ResultsHelper.Problem);
                })
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces<ApiErrorResponse>(StatusCodes.Status404NotFound)
            .Produces<ApiErrorResponse>(StatusCodes.Status422UnprocessableEntity)
            .HasApiVersion(1.0)
            .WithName(SettingsEndpoints.GetOpenApiName(nameof(SettingsEndpoints.WashingMachineEndpoints.UpdateWashingMachine)));

        return builder;
    }
}
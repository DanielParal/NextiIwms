using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Nexticz.Module.Mmo.Settings.Contracts.Depositors;
using Nexticz.Lib.Shared.Errors.Models;
using Nexticz.Lib.Shared.Helpers;
using Nexticz.Module.Mmo.Settings.Application.Depositors;
using Nexticz.Module.Mmo.Settings.Application.Depositors.Commands.CreateDepositor;


namespace Nexticz.Module.Mmo.Settings.Presentation.Depositors;

internal static class CreateDepositorEndpoint
{
    public static IEndpointRouteBuilder MapCreateDepositor(this IEndpointRouteBuilder builder)
    {
        builder.MapPost(SettingsEndpoints.DepositorEndpoints.CreateDepositor,
                async (
                    CreateDepositorRequest request,
                    ISender mediator,
                    CancellationToken cancellationToken) =>
                {
                    var createDepositorCommand = new CreateDepositorCommand(request.Code, request.Name, request.BarcodeTemplate);
                    var result = await mediator.Send(createDepositorCommand, cancellationToken);

                    return result.Match(
                        depositor => Results.Created(
                            $"{SettingsEndpoints.DepositorEndpoints.GetDepositors}/{depositor.Code}",
                            DepositorResponseFactory.Create(depositor)),
                        ResultsHelper.Problem);
                })
            .Produces<DepositorResponse>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces<ApiErrorResponse>(StatusCodes.Status422UnprocessableEntity)
            .HasApiVersion(1.0)
            .WithName(SettingsEndpoints.GetOpenApiName(nameof(SettingsEndpoints.DepositorEndpoints.CreateDepositor)));

        return builder;
    }
}
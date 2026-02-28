using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Nexticz.Module.Sign.Settings.Contracts.Depositors;
using Nexticz.Lib.Shared.Errors.Models;
using Nexticz.Lib.Shared.Helpers;
using Nexticz.Module.Sign.Settings.Application.Depositors;
using Nexticz.Module.Sign.Settings.Application.Depositors.Commands.CreateDepositor;

namespace Nexticz.Module.Sign.Settings.Presentation.Depositors;

internal static class CreateDepositorEndpoint
{
    public static IEndpointRouteBuilder MapCreateDepositorEndpoint(this IEndpointRouteBuilder builder)
    {
        builder.MapPost(SettingsEndpoints.DepositorEndpoints.CreateDepositor,
                async (
                    CreateDepositorRequest request,
                    ISender sender,
                    CancellationToken cancellationToken) =>
                {
                    var result = 
                        await sender.Send(new CreateDepositorCommand(
                            request.Code, request.Name, request.DepositorGroupCode, request.DeliveryTemplateCode, request.LoadingTemplateCode), 
                            cancellationToken);

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
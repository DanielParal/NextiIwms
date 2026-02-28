using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Nexticz.Module.Sign.Settings.Contracts.Depositors;
using Nexticz.Lib.Shared.Errors.Models;
using Nexticz.Lib.Shared.Helpers;
using Nexticz.Module.Sign.Settings.Application.Depositors.Commands.UpdateDepositor;

namespace Nexticz.Module.Sign.Settings.Presentation.Depositors;

internal static class UpdateDepositorEndpoint
{
    public static IEndpointRouteBuilder MapUpdateDepositorEndpoint(this IEndpointRouteBuilder builder)
    {
        builder.MapPut(SettingsEndpoints.DepositorEndpoints.UpdateDepositor,
                async (
                    string code, 
                    UpdateDepositorRequest request, 
                    ISender sender,
                    CancellationToken cancellationToken) =>
                {
                    var result = 
                        await sender.Send(new UpdateDepositorCommand(
                            code, request.Name, request.DepositorGroupCode, request.DeliveryTemplateCode, request.LoadingTemplateCode), 
                            cancellationToken);

                    return result.Match(
                        _ => Results.NoContent(),
                        ResultsHelper.Problem);
                })
            .Produces<object>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces<ApiErrorResponse>(StatusCodes.Status404NotFound)
            .Produces<ApiErrorResponse>(StatusCodes.Status422UnprocessableEntity)
            .HasApiVersion(1.0)
            .WithName(SettingsEndpoints.GetOpenApiName(nameof(SettingsEndpoints.DepositorEndpoints.UpdateDepositor)));
        
        return builder;
    }
}
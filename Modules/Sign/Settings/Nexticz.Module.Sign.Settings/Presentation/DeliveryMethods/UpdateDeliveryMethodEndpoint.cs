using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Nexticz.Module.Sign.Settings.Contracts.DeliveryMethods;
using Nexticz.Lib.Shared.Errors.Models;
using Nexticz.Lib.Shared.Helpers;
using Nexticz.Module.Sign.Settings.Application.DeliveryMethods.Commands.UpdateDeliveryMethod;

namespace Nexticz.Module.Sign.Settings.Presentation.DeliveryMethods;

internal static class UpdateDeliveryMethodEndpoint
{
    public static IEndpointRouteBuilder MapUpdateDeliveryMethodEndpoint(this IEndpointRouteBuilder builder)
    {
        builder.MapPut(SettingsEndpoints.DeliveryMethodEndpoints.UpdateDeliveryMethod,
                async (
                    string code, 
                    UpdateDeliveryMethodRequest request, 
                    ISender sender,
                    CancellationToken cancellationToken) =>
                {
                    var result = 
                        await sender.Send(
                            new UpdateDeliveryMethodCommand(code, request.Name, request.LoadingDocumentPrintCopiesCount, request.DeliveryDocumentPrintCopiesCount), 
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
            .WithName(SettingsEndpoints.GetOpenApiName(nameof(SettingsEndpoints.DeliveryMethodEndpoints.UpdateDeliveryMethod)));
        
        return builder;
    }
}
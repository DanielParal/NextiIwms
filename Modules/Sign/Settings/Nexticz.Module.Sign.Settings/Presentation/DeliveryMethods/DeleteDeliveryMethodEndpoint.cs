using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Nexticz.Lib.Shared.Helpers;
using Nexticz.Module.Sign.Settings.Application.DeliveryMethods.Commands.DeleteDeliveryMethod;

namespace Nexticz.Module.Sign.Settings.Presentation.DeliveryMethods;

internal static class DeleteDeliveryMethodEndpoint
{
    public static IEndpointRouteBuilder MapDeleteDepositorGroupEndpoint(this IEndpointRouteBuilder builder)
    {
        builder.MapDelete(SettingsEndpoints.DeliveryMethodEndpoints.DeleteDeliveryMethod,
                async (
                    string code, 
                    ISender mediatr,
                    CancellationToken cancellationToken) =>
                {
                    var result = await mediatr.Send(new DeleteDeliveryMethodCommand(code), cancellationToken);

                    return result.Match(
                        _ => Results.NoContent(),
                        ResultsHelper.Problem);
                })
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status404NotFound)
            .HasApiVersion(1.0)
            .WithName(SettingsEndpoints.GetOpenApiName(nameof(SettingsEndpoints.DeliveryMethodEndpoints.DeleteDeliveryMethod)));

        return builder;
    }
}
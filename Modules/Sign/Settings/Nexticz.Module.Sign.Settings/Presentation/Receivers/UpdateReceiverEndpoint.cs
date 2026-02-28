using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Nexticz.Module.Sign.Settings.Contracts.Receivers;
using Nexticz.Lib.Shared.Errors.Models;
using Nexticz.Lib.Shared.Helpers;
using Nexticz.Module.Sign.Settings.Application.Receivers.Commands.UpdateReceiver;

namespace Nexticz.Module.Sign.Settings.Presentation.Receivers;

internal static class UpdateReceiverEndpoint
{
    public static IEndpointRouteBuilder MapUpdateReceiverEndpoint(this IEndpointRouteBuilder builder)
    {
        builder.MapPut(SettingsEndpoints.ReceiverEndpoints.UpdateReceiver,
                async (
                    string code, 
                    string partnerCode,
                    UpdateReceiverRequest request, 
                    ISender sender,
                    CancellationToken cancellationToken) =>
                {
                    var result = 
                        await sender.Send(new UpdateReceiverCommand(code, partnerCode, request.Name), cancellationToken);

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
            .WithName(SettingsEndpoints.GetOpenApiName(nameof(SettingsEndpoints.ReceiverEndpoints.UpdateReceiver)));
        
        return builder;
    }
}
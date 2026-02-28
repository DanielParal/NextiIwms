using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Nexticz.Lib.Shared.Helpers;
using Nexticz.Module.Sign.Settings.Application.Receivers.Commands.DeleteReceiver;

namespace Nexticz.Module.Sign.Settings.Presentation.Receivers;

internal static class DeleteReceiverEndpoint
{
    public static IEndpointRouteBuilder MapDeleteReceiverEndpoint(this IEndpointRouteBuilder builder)
    {
        builder.MapDelete(SettingsEndpoints.ReceiverEndpoints.DeleteReceiver,
                async (
                    string code,
                    string partnerCode,
                    ISender mediatr,
                    CancellationToken cancellationToken) =>
                {
                    var result = await mediatr.Send(new DeleteReceiverCommand(code, partnerCode), cancellationToken);

                    return result.Match(
                        _ => Results.NoContent(),
                        ResultsHelper.Problem);
                })
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status404NotFound)
            .HasApiVersion(1.0)
            .WithName(SettingsEndpoints.GetOpenApiName(nameof(SettingsEndpoints.ReceiverEndpoints.DeleteReceiver)));

        return builder;
    }
}
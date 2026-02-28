using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Nexticz.Module.Sign.Settings.Contracts.Receivers;
using Nexticz.Lib.Shared.Errors.Models;
using Nexticz.Lib.Shared.Helpers;
using Nexticz.Module.Sign.Settings.Application.Receivers.Commands.CreateReceiver;

namespace Nexticz.Module.Sign.Settings.Presentation.Receivers;

internal static class CreateReceiverEndpoint
{
    public static IEndpointRouteBuilder MapCreateReceiverEndpoint(this IEndpointRouteBuilder builder)
    {
        builder.MapPost(SettingsEndpoints.ReceiverEndpoints.CreateReceiver,
                async (
                    CreateReceiverRequest request,
                    ISender sender,
                    CancellationToken cancellationToken) =>
                {
                    var result = 
                        await sender.Send(new CreateReceiverCommand(request.Code, request.PartnerCode, request.Name), cancellationToken);

                    if (result.IsError)
                        return ResultsHelper.Problem(result.Errors);
                    
                    return result.Match(
                        receiver => Results.Created(
                            $"{SettingsEndpoints.ReceiverEndpoints.CreateReceiver}/{receiver.Code}/{receiver.PartnerCode}",
                            ReceiverResponseFactory.Create(receiver)),
                        ResultsHelper.Problem);
                })
            .Produces<ReceiverResponse>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces<ApiErrorResponse>(StatusCodes.Status422UnprocessableEntity)
            .HasApiVersion(1.0)
            .WithName(SettingsEndpoints.GetOpenApiName(nameof(SettingsEndpoints.ReceiverEndpoints.CreateReceiver)));

        return builder;
    }
}
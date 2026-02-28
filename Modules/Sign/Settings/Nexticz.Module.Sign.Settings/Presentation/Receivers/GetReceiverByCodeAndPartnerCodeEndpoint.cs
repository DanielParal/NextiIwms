using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Nexticz.Module.Sign.Settings.Application.Partners.Queries.GetPartnerByCode;
using Nexticz.Module.Sign.Settings.Contracts.Receivers;
using Nexticz.Lib.Shared.Helpers;
using Nexticz.Module.Sign.Settings.Application.Receivers.Queries.GetReceiverByCodeAndPartnerCode;

namespace Nexticz.Module.Sign.Settings.Presentation.Receivers;

internal static class GetReceiverByCodeAndPartnerCodeEndpoint
{
    public static IEndpointRouteBuilder MapGetReceiverByCodeAndPartnerCodeEndpoint(this IEndpointRouteBuilder builder)
    {
        builder.MapGet(SettingsEndpoints.ReceiverEndpoints.GetReceiverByCodeAndPartnerCode,
                async (
                    string code, 
                    string partnerCode,
                    ISender sender,
                    CancellationToken cancellationToken
                ) =>
                {
                    var result = 
                        await sender.Send(new GetReceiverByCodeAndPartnerCodeQuery(code, partnerCode), cancellationToken);

                    return result.Match(
                        receiver => Results.Ok(ReceiverResponseFactory.Create(receiver)),
                        ResultsHelper.Problem);
                })
            .Produces<ReceiverResponse>()
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status404NotFound)
            .HasApiVersion(1.0)
            .WithName(SettingsEndpoints.GetOpenApiName(nameof(SettingsEndpoints.ReceiverEndpoints.GetReceiverByCodeAndPartnerCode)));

        return builder;
    }
}
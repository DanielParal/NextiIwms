using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Nexticz.Module.Sign.Settings.Contracts.DeliveryMethods;
using Nexticz.Lib.Shared.Helpers;
using Nexticz.Module.Sign.Settings.Application.DeliveryMethods.Queries.GetDeliveryMethodByCode;

namespace Nexticz.Module.Sign.Settings.Presentation.DeliveryMethods;

internal static class GetDeliveryMethodByCodeEndpoint
{
    public static IEndpointRouteBuilder MapGetDeliveryMethodByCodeEndpoint(this IEndpointRouteBuilder builder)
    {
        builder.MapGet(SettingsEndpoints.DeliveryMethodEndpoints.GetDeliveryMethodByCode,
                async (
                    string code, 
                    ISender sender,
                    CancellationToken cancellationToken
                ) =>
                {
                    var result = 
                        await sender.Send(new GetDeliveryMethodByCodeQuery(code), cancellationToken);
                    
                    return result.Match(
                        depositor => Results.Ok(DeliveryMethodResponseFactory.Create(depositor)),
                        ResultsHelper.Problem);
                })
            .Produces<DeliveryMethodResponse>()
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status404NotFound)
            .HasApiVersion(1.0)
            .WithName(SettingsEndpoints.GetOpenApiName(nameof(SettingsEndpoints.DeliveryMethodEndpoints.GetDeliveryMethodByCode)));

        return builder;
    }
}
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Nexticz.Module.Sign.Settings.Contracts.DeliveryMethods;
using Nexticz.Lib.Shared.Errors.Models;
using Nexticz.Lib.Shared.Helpers;
using Nexticz.Module.Sign.Settings.Application.DeliveryMethods.Commands.CreateDeliveryMethod;


namespace Nexticz.Module.Sign.Settings.Presentation.DeliveryMethods;

internal static class CreateDeliveryMethodEndpoint
{
    public static IEndpointRouteBuilder MapCreateDeliveryMethodEndpoint(this IEndpointRouteBuilder builder)
    {
        builder.MapPost(SettingsEndpoints.DeliveryMethodEndpoints.CreateDeliveryMethod,
                async (
                    CreateDeliveryMethodRequest request,
                    ISender sender,
                    CancellationToken cancellationToken) =>
                {
                    var result = 
                        await sender.Send(new CreateDeliveryMethodCommand(request.Code, request.Name, request.LoadingDocumentPrintCopiesCount, request.DeliveryDocumentPrintCopiesCount),
                            cancellationToken);

                    return result.Match(
                        depositorGroup => Results.Created(
                            $"{SettingsEndpoints.DeliveryMethodEndpoints.GetDeliveryMethods}/{depositorGroup.Code}",
                            DeliveryMethodResponseFactory.Create(depositorGroup)),
                        ResultsHelper.Problem);
                })
            .Produces<DeliveryMethodResponse>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces<ApiErrorResponse>(StatusCodes.Status422UnprocessableEntity)
            .HasApiVersion(1.0)
            .WithName(SettingsEndpoints.GetOpenApiName(nameof(SettingsEndpoints.DeliveryMethodEndpoints.CreateDeliveryMethod)));

        return builder;
    }
}
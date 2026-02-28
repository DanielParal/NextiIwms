using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Nexticz.Module.Sign.Settings.Contracts.Partners;
using Nexticz.Lib.Shared.Errors.Models;
using Nexticz.Lib.Shared.Helpers;
using Nexticz.Module.Sign.Settings.Application.Partners.Commands.UpdatePartner;


namespace Nexticz.Module.Sign.Settings.Presentation.Partners;

internal static class UpdatePartnerEndpoint
{
    public static IEndpointRouteBuilder MapUpdatePartnerEndpoint(this IEndpointRouteBuilder builder)
    {
        builder.MapPut(SettingsEndpoints.PartnerEndpoints.UpdatePartner,
                async (
                    string code, 
                    UpdatePartnerRequest request, 
                    ISender sender,
                    CancellationToken cancellationToken) =>
                {
                    var result = 
                        await sender.Send(new UpdatePartnerCommand(code, request.Name), cancellationToken);

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
            .WithName(SettingsEndpoints.GetOpenApiName(nameof(SettingsEndpoints.PartnerEndpoints.UpdatePartner)));
        
        return builder;
    }
}
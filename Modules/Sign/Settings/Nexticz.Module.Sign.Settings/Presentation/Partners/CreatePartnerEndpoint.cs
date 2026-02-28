using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Nexticz.Module.Sign.Settings.Contracts.Partners;
using Nexticz.Lib.Shared.Errors.Models;
using Nexticz.Lib.Shared.Helpers;
using Nexticz.Module.Sign.Settings.Application.Partners.Commands.CreatePartner;


namespace Nexticz.Module.Sign.Settings.Presentation.Partners;

internal static class CreatePartnerEndpoint
{
    public static IEndpointRouteBuilder MapCreatePartnerEndpoint(this IEndpointRouteBuilder builder)
    {
        builder.MapPost(SettingsEndpoints.PartnerEndpoints.CreatePartner,
                async (
                    CreatePartnerRequest request,
                    ISender sender,
                    CancellationToken cancellationToken) =>
                {
                    var result = 
                        await sender.Send(new CreatePartnerCommand(request.Code, request.Name), cancellationToken);

                    return result.Match(
                        partner => Results.Created(
                            $"{SettingsEndpoints.PartnerEndpoints.GetPartners}/{partner.Code}",
                            PartnerResponseFactory.Create(partner)),
                        ResultsHelper.Problem);
                })
            .Produces<PartnerResponse>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces<ApiErrorResponse>(StatusCodes.Status422UnprocessableEntity)
            .HasApiVersion(1.0)
            .WithName(SettingsEndpoints.GetOpenApiName(nameof(SettingsEndpoints.PartnerEndpoints.CreatePartner)));

        return builder;
    }
}
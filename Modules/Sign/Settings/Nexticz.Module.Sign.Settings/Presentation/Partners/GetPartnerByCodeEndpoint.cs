using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Nexticz.Module.Sign.Settings.Contracts.Partners;
using Nexticz.Lib.Shared.Helpers;
using Nexticz.Module.Sign.Settings.Application.Partners.Queries.GetPartnerByCode;

namespace Nexticz.Module.Sign.Settings.Presentation.Partners;

internal static class GetPartnerByCodeEndpoint
{
    public static IEndpointRouteBuilder MapGetPartnerByCodeEndpoint(this IEndpointRouteBuilder builder)
    {
        builder.MapGet(SettingsEndpoints.PartnerEndpoints.GetPartnerByCode,
                async (
                    string code, 
                    ISender sender,
                    CancellationToken cancellationToken
                ) =>
                {
                    var result = 
                        await sender.Send(new GetPartnerByCodeQuery(code), cancellationToken);
                    
                    return result.Match(
                        depositor => Results.Ok(PartnerResponseFactory.Create(depositor)),
                        ResultsHelper.Problem);
                })
            .Produces<PartnerResponse>()
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status404NotFound)
            .HasApiVersion(1.0)
            .WithName(SettingsEndpoints.GetOpenApiName(nameof(SettingsEndpoints.PartnerEndpoints.GetPartnerByCode)));

        return builder;
    }
}
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Nexticz.Module.Cuzk.Contracts.Municipalities;
using Nexticz.Lib.Shared.Helpers;
using Nexticz.Module.Cuzk.Application.Municipalities.Queries.GetMunicipalityByCode;

namespace Nexticz.Module.Cuzk.Presentation.Municipalities;

internal static class GetMunicipalityByCodeEndpoint
{
    public static IEndpointRouteBuilder MapGetMunicipalityByCodeEndpoint(this IEndpointRouteBuilder builder)
    {
        builder.MapGet(CuzkEndpoints.MunicipalityEndpoints.GetMunicipalityByCode,
                async (
                    string code, 
                    ISender sender,
                    CancellationToken cancellationToken
                ) =>
                {
                    var result = 
                        await sender.Send(new GetMunicipalityByCodeQuery(code), cancellationToken);
                    
                    return result.Match(
                        municipality => Results.Ok(MunicipalityResponseFactory.Create(municipality)),
                        ResultsHelper.Problem);
                })
            .Produces<MunicipalityResponse>()
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status404NotFound)
            .HasApiVersion(1.0)
            .WithName(CuzkEndpoints.GetOpenApiName(nameof(CuzkEndpoints.MunicipalityEndpoints.GetMunicipalityByCode)));

        return builder;
    }
}
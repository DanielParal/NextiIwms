using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Nexticz.Module.Cuzk.Contracts.EconomicSubjects;
using Nexticz.Module.Cuzk.Application.EconomicSubjects.Queries.GetEconomicSubjectByIco;

namespace Nexticz.Module.Cuzk.Presentation.EconomicSubjects;

public static class GetEconomicSubjectByIcoEndpoint
{
    public static IEndpointRouteBuilder MapGetEconomicSubjectByIcoEndpoint(this IEndpointRouteBuilder builder)
    {
        builder.MapGet(CuzkEndpoints.EconomicSubjectEndpoints.Get,
                async (string ico, ISender sender, CancellationToken cancellationToken) =>
                {
                    var result = await sender.Send(new GetEconomicSubjectByIcoQuery(ico), cancellationToken);
                    
                    return Results.Ok(result);
                })
            .Produces<EconomicSubjectResponse?>()
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .HasApiVersion(1.0)
            .WithName(nameof(CuzkEndpoints.EconomicSubjectEndpoints.Get));
        
        return builder;
    }
}
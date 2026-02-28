using Microsoft.AspNetCore.Routing;

namespace Nexticz.Module.Cuzk.Presentation.EconomicSubjects;

public static class EconomicSubjectsExtensions
{
    public static IEndpointRouteBuilder MapEconomicSubjectsEndpoints(this IEndpointRouteBuilder builder)
    {
        return builder
            .MapGetEconomicSubjectByIcoEndpoint();
    }
}
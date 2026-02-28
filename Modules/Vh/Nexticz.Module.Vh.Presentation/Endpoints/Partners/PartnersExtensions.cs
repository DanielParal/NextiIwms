using Microsoft.AspNetCore.Routing;

namespace Nexticz.Module.Vh.Presentation.Endpoints.Partners;

public static class PartnersExtensions
{
    public static IEndpointRouteBuilder MapPartnersEndpoints(this IEndpointRouteBuilder builder)
    {
        return builder
            .MapCreatePartner()
            .MapUpdatePartner()
            .MapDeletePartner()
            .MapGetPartnerById()
            .MapGetPartners();
    }
}
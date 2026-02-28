using Microsoft.AspNetCore.Routing;

namespace Nexticz.Module.Sign.Settings.Presentation.Partners;

internal static class PartnerEndpointsExtensions
{
    public static IEndpointRouteBuilder MapPartnersEndpoints(this IEndpointRouteBuilder builder)
    {
        return builder
            .MapCreatePartnerEndpoint()
            .MapGetPartnerByCodeEndpoint()
            .MapGetPartnersEndpoint()
            .MapUpdatePartnerEndpoint()
            .MapDeletePartnerEndpoint();
    }
}
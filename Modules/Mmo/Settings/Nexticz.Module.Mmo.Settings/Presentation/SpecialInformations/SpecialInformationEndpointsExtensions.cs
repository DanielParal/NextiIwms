using Microsoft.AspNetCore.Routing;

namespace Nexticz.Module.Mmo.Settings.Presentation.SpecialInformations;

internal static class SpecialInformationEndpointsExtensions
{
    public static IEndpointRouteBuilder MapSpecialInformationsEndpoints(this IEndpointRouteBuilder builder)
    {
        return builder
            .MapCreateSpecialInformationEndpoint()
            .MapDeleteSpecialInformationEndpoint()
            .MapDeleteSpecialInformationFileEndpoint()
            .MapUpdateSpecialInformationEndpoint()
            .MapUploadSpecialInformationFileEndpoint()
            .MapGetSpecialInformationFileEndpoint()
            .MapGetSpecialInformationByIdEndpoint()
            .MapGetSpecialInformationsEndpoint();
    }
}
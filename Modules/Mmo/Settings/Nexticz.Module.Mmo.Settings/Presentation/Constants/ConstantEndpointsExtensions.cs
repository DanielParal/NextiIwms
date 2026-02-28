using Microsoft.AspNetCore.Routing;

namespace Nexticz.Module.Mmo.Settings.Presentation.Constants;

internal static class ConstantEndpointsExtensions
{
    public static IEndpointRouteBuilder MapConstantsEndpoints(this IEndpointRouteBuilder builder)
    {
        return builder
            .MapGetConstants()
            .MapGetConstantByKey();
    }
    
    public static IEndpointRouteBuilder MapDeveloperOnlyConstantsEndpoints(this IEndpointRouteBuilder builder)
    {
        return builder
            .MapUpdateConstant()
            .MapCreateConstant()
            .MapDeleteConstant();
    }
}
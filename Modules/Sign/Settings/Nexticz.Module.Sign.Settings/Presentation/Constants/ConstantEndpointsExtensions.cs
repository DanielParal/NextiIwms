using Microsoft.AspNetCore.Routing;

namespace Nexticz.Module.Sign.Settings.Presentation.Constants;

internal static class ConstantEndpointsExtensions
{
    public static IEndpointRouteBuilder MapConstantsEndpoints(this IEndpointRouteBuilder builder)
    {
        return builder
            .MapGetConstants()
            .MapGetConstantByKey()
            .MapUpdateConstant();
    }
    
    public static IEndpointRouteBuilder MapDeveloperOnlyConstantsEndpoints(this IEndpointRouteBuilder builder)
    {
        return builder
            .MapCreateConstant()
            .MapDeleteConstant();
    }
}
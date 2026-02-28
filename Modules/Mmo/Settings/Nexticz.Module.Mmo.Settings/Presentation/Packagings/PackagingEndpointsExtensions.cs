using Microsoft.AspNetCore.Routing;

namespace Nexticz.Module.Mmo.Settings.Presentation.Packagings;

internal static class PackagingEndpointsExtensions
{
    public static IEndpointRouteBuilder MapPackagingsEndpoints(this IEndpointRouteBuilder builder)
    {
        return builder
            .MapCreatePackaging()
            .MapUpdatePackaging()
            .MapDeletePackaging()
            .MapGetPackagingById();
    }
    
    public static IEndpointRouteBuilder MapAnyPermissionPackagingsEndpoints(this IEndpointRouteBuilder builder)
    {
        return builder
            .MapGetPackagings();
    }
}
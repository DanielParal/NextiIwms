using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Nexticz.Module.Cuzk.Presentation.AddressLocations;
using Nexticz.Module.Cuzk.Presentation.AddressLocationSlugs;
using Nexticz.Module.Cuzk.Presentation.EconomicSubjects;
using Nexticz.Module.Cuzk.Presentation.Imports;
using Nexticz.Module.Cuzk.Presentation.Municipalities;
using Nexticz.Module.Cuzk.Presentation.OpenApiContracts;
using Nexticz.Lib.Shared.Helpers;

namespace Nexticz.Module.Cuzk.Presentation;

internal static class EndpointsExtensions
{
    public static IEndpointRouteBuilder MapCuzkMunicipalitiesEndpoints(this IEndpointRouteBuilder builder)
    {
        builder.NewVersionedApi(nameof(CuzkEndpoints.MunicipalityEndpoints))
            .RequireAuthorization(policy =>
                policy.RequireAssertion(context => AuthorizationHelper.AddRequiredClaimsToHttpContextItems(
                    context,
                    [nameof(AuthorizationHelper.Role.Developer), nameof(AuthorizationHelper.Role.SysAdmin)],
                    [nameof(AuthorizationHelper.Permission.Any)])
                )
            )
            .MapMunicipalitiesEndpoints();

        return builder;
    }
    
    public static IEndpointRouteBuilder MapCuzkAddressLocationsEndpoints(this IEndpointRouteBuilder builder)
    {
        builder.NewVersionedApi(nameof(CuzkEndpoints.AddressLocationEndpoints))
            .RequireAuthorization(policy =>
                policy.RequireAssertion(context => AuthorizationHelper.AddRequiredClaimsToHttpContextItems(
                    context,
                    [nameof(AuthorizationHelper.Role.Developer), nameof(AuthorizationHelper.Role.SysAdmin)],
                    [nameof(AuthorizationHelper.Permission.Any)])
                )
            )
            .MapAddressLocationsEndpoints();

        return builder;
    }
    
    public static IEndpointRouteBuilder MapCuzkAddressLocationSlugsEndpoints(this IEndpointRouteBuilder builder)
    {
        builder.NewVersionedApi(nameof(CuzkEndpoints.AddressLocationSlugEndpoints))
            .RequireAuthorization(policy =>
                policy.RequireAssertion(context => AuthorizationHelper.AddRequiredClaimsToHttpContextItems(
                    context,
                    [nameof(AuthorizationHelper.Role.Developer), nameof(AuthorizationHelper.Role.SysAdmin)],
                    [nameof(AuthorizationHelper.Permission.Any)])
                )
            )
            .MapAddressLocationSlugsEndpoints();

        return builder;
    }
    
    public static IEndpointRouteBuilder MapCuzkEconomicSubjectsEndpoints(this IEndpointRouteBuilder builder)
    {
        builder.NewVersionedApi(nameof(CuzkEndpoints.EconomicSubjectEndpoints))
            .RequireAuthorization(policy =>
                policy.RequireAssertion(context => AuthorizationHelper.AddRequiredClaimsToHttpContextItems(
                    context,
                    [nameof(AuthorizationHelper.Role.Developer), nameof(AuthorizationHelper.Role.SysAdmin)],
                    [nameof(AuthorizationHelper.Permission.Any)])
                )
            )
            .MapEconomicSubjectsEndpoints();

        return builder;
    }
    
    public static IEndpointRouteBuilder MapCuzkImportsEndpoints(this IEndpointRouteBuilder builder)
    {
        builder.NewVersionedApi(nameof(CuzkEndpoints.ImportEndpoints))
            .RequireAuthorization(policy =>
                policy.RequireAssertion(context => AuthorizationHelper.AddRequiredClaimsToHttpContextItems(
                    context,
                    [nameof(AuthorizationHelper.Role.Developer), nameof(AuthorizationHelper.Role.SysAdmin)],
                    [nameof(AuthorizationHelper.Permission.Any)])
                )
            )
            .MapImportsEndpoints();

        return builder;
    }
    
    public static IEndpointRouteBuilder MapCuzkOpenApiContractEndpoints(this IEndpointRouteBuilder builder)
    {
        builder.NewVersionedApi(nameof(CuzkEndpoints.OpenApiContractEndpoints))
            .RequireAuthorization(policy =>
                policy.RequireAssertion(context => AuthorizationHelper.AddRequiredClaimsToHttpContextItems(
                    context,
                    [nameof(AuthorizationHelper.Role.Developer)],
                    [nameof(AuthorizationHelper.Permission.Any)])
                )
            )
            .MapOpenApiContractEndpoints();

        return builder;
    }
}
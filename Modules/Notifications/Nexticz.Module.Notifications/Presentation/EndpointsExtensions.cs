using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Nexticz.Module.Notifications.Presentation.OpenApiContracts;
using Nexticz.Lib.Shared.Helpers;
using Nexticz.Module.Notifications.Presentation.Hubs;

namespace Nexticz.Module.Notifications.Presentation;

internal static class EndpointsExtensions
{
    public static IEndpointRouteBuilder MapNotificationsOpenApiContractEndpoints(this IEndpointRouteBuilder builder)
    {
        builder.NewVersionedApi(nameof(NotificationsEndpoints.OpenApiContractEndpoints))
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
    
    public static IEndpointRouteBuilder MapNotificationsHubEndpointsExtensions(this IEndpointRouteBuilder builder)
    {
        builder.NewVersionedApi(nameof(NotificationsEndpoints.OpenApiContractEndpoints))
            .RequireAuthorization(policy =>
                policy.RequireAssertion(context => AuthorizationHelper.AddRequiredClaimsToHttpContextItems(
                    context,
                    [nameof(AuthorizationHelper.Role.Any)],
                    [nameof(AuthorizationHelper.Permission.Any)])
                )
            )
            .MapHubEndpointsExtensions();

        return builder;
    }
}
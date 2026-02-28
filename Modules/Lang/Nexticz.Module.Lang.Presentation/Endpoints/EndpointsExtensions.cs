using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Nexticz.Module.Lang.Presentation.Endpoints.Languages;
using Nexticz.Module.Lang.Presentation.Endpoints.Translations;
using Nexticz.Lib.Shared.Helpers;

namespace Nexticz.Module.Lang.Presentation.Endpoints;

public static class EndpointsExtensions
{
    public static IEndpointRouteBuilder MapLangApiEndpoints(this IEndpointRouteBuilder builder)
    {
        builder.NewVersionedApi(nameof(ApiEndpoints.Languages))
            .RequireAuthorization(policy => policy.RequireAssertion(context =>
                    AuthorizationHelper.AddRequiredClaimsToHttpContextItems(
                        context,
                        [nameof(AuthorizationHelper.Role.Developer), nameof(AuthorizationHelper.Role.SysAdmin)],
                        [nameof(AuthorizationHelper.Permission.Any)])
                )
            )
            .MapLanguagesEndpoints();

        builder.NewVersionedApi(nameof(ApiEndpoints.Translations))
            .RequireAuthorization(policy => policy.RequireAssertion(context =>
                    AuthorizationHelper.AddRequiredClaimsToHttpContextItems(
                        context,
                        [nameof(AuthorizationHelper.Role.Developer), nameof(AuthorizationHelper.Role.SysAdmin)],
                        [nameof(AuthorizationHelper.Permission.Any)])
                )
            )
            .MapTranslationsEndpoints();

        return builder;
    }
}
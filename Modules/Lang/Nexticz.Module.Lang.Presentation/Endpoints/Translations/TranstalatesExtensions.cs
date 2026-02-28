using Microsoft.AspNetCore.Routing;

namespace Nexticz.Module.Lang.Presentation.Endpoints.Translations;

public static class TranstalationsExtensions
{
    public static IEndpointRouteBuilder MapTranslationsEndpoints(this IEndpointRouteBuilder builder)
    {
        builder.MapGetTranslationById();
        builder.MapGetTranslations();
        builder.MapCreateTranslations();
        builder.MapUpdateTranslation();
        builder.MapRemoveTranslation();

        return builder;
    }
}
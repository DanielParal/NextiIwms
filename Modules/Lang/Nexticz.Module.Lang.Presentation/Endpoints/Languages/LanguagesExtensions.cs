using Microsoft.AspNetCore.Routing;

namespace Nexticz.Module.Lang.Presentation.Endpoints.Languages;

public static class LanguagesExtensions
{
    public static IEndpointRouteBuilder MapLanguagesEndpoints(this IEndpointRouteBuilder builder)
    {
        builder.MapGetLanguageByShortcut();
        builder.MapGetLanguages();
        builder.MapUpdateLanguage();

        return builder;
    }
}
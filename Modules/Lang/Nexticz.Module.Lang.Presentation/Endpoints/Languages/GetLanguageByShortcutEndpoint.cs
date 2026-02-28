using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Nexticz.Module.Lang.Application.Languages.Queries.GetLanguageByShortcut;
using Nexticz.Module.Lang.Contracts.Languages;
using Nexticz.Lib.Shared.Errors.Models;
using Nexticz.Lib.Shared.Helpers;


namespace Nexticz.Module.Lang.Presentation.Endpoints.Languages;

public static class GetLanguageByShortcutEndpoint
{
    public static IEndpointRouteBuilder MapGetLanguageByShortcut(this IEndpointRouteBuilder builder)
    {
        builder.MapGet(ApiEndpoints.Languages.GetLanguageByShortcut, async (EnumHelper.LanguageShortcutEnum shortcut,
                ISender mediatr, CancellationToken cancellationToken) =>
            {
                var query = new GetLanguageByShortcutQuery { Shortcut = shortcut };

                var result = await mediatr.Send(query, cancellationToken);

                return result.Match(
                    Results.Ok,
                    ResultsHelper.Problem);
            })
            .Produces<LanguageResponse>()
            .Produces<ApiErrorResponse>(StatusCodes.Status404NotFound)
            .HasApiVersion(1.0)
            .WithName(nameof(ApiEndpoints.Languages.GetLanguageByShortcut));

        return builder;
    }
}
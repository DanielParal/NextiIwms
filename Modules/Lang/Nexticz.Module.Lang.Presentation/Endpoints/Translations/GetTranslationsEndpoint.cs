using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Nexticz.Module.Lang.Application.Translations.Common.Models;
using Nexticz.Module.Lang.Application.Translations.Queries.ListTranslations;
using Nexticz.Module.Lang.Contracts.Translations;
using Nexticz.Lib.Shared.DevExtreme;
using Nexticz.Lib.Shared.Helpers;


namespace Nexticz.Module.Lang.Presentation.Endpoints.Translations;

public static class GetTranslationsEndpoint
{
    public static IEndpointRouteBuilder MapGetTranslations(this IEndpointRouteBuilder builder)
    {
        builder.MapGet(ApiEndpoints.Translations.GetTranslations,
                async ([AsParameters] TranslationsFilteringParams filteringParams,
                    ISender mediatr, CancellationToken cancellationToken) =>
                {
                    var query = new ListTranslationsQuery { FilteringParams = filteringParams };
                    var result = await mediatr.Send(query, cancellationToken);

                    return result.Match(
                        Results.Ok,
                        ResultsHelper.Problem);
                })
            .Produces<FilteredResult<TranslationResponse>>()
            .HasApiVersion(1.0)
            .WithName(nameof(ApiEndpoints.Translations.GetTranslations))
            .AllowAnonymous();

        return builder;
    }
}
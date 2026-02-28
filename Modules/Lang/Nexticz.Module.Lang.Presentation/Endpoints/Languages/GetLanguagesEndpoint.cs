using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Nexticz.Module.Lang.Application.Languages.Common.Models;
using Nexticz.Module.Lang.Application.Languages.Queries.ListLanguages;
using Nexticz.Module.Lang.Contracts.Languages;
using Nexticz.Lib.Shared.DevExtreme;
using Nexticz.Lib.Shared.Helpers;


namespace Nexticz.Module.Lang.Presentation.Endpoints.Languages;

public static class GetLanguagesEndpoint
{
    public static IEndpointRouteBuilder MapGetLanguages(this IEndpointRouteBuilder builder)
    {
        builder.MapGet(ApiEndpoints.Languages.GetLanguages,
                async ([AsParameters] LanguagesFilteringParams filteringParams,
                    ISender mediatr, CancellationToken cancellationToken) =>
                {
                    var query = new ListLanguagesQuery { FilteringParams = filteringParams };
                    var result = await mediatr.Send(query, cancellationToken);

                    return result.Match(
                        Results.Ok,
                        ResultsHelper.Problem);
                })
            .HasApiVersion(1.0)
            .Produces<FilteredResult<LanguageResponse>>()
            .WithName(nameof(ApiEndpoints.Languages.GetLanguages))
            .AllowAnonymous();

        return builder;
    }
}
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Nexticz.Module.Lang.Application.Translations.Queries.GetTranslationById;
using Nexticz.Module.Lang.Contracts.Translations;
using Nexticz.Lib.Shared.Errors.Models;
using Nexticz.Lib.Shared.Helpers;


namespace Nexticz.Module.Lang.Presentation.Endpoints.Translations;

public static class GetTranslationByIdEndpoint
{
    public static IEndpointRouteBuilder MapGetTranslationById(this IEndpointRouteBuilder builder)
    {
        builder.MapGet(ApiEndpoints.Translations.GetTranslationById,
                async (Guid id, ISender mediatr, CancellationToken cancellationToken) =>
                {
                    var query = new GetTranslationByIdQuery { Id = id };
                    var result = await mediatr.Send(query, cancellationToken);

                    return result.Match(
                        Results.Ok,
                        ResultsHelper.Problem);
                })
            .Produces<TranslationResponse>()
            .Produces<ApiErrorResponse>(StatusCodes.Status404NotFound)
            .HasApiVersion(1.0)
            .WithName(nameof(ApiEndpoints.Translations.GetTranslationById));

        return builder;
    }
}
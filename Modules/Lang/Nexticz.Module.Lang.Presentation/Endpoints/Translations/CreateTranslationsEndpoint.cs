using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Nexticz.Module.Lang.Application.Translations.Commands.CreateTranslations;
using Nexticz.Module.Lang.Contracts.Translations;
using Nexticz.Lib.Shared.Errors.Models;
using Nexticz.Lib.Shared.Helpers;


namespace Nexticz.Module.Lang.Presentation.Endpoints.Translations;

public static class CreateTranslationsEndpoint
{
    public static IEndpointRouteBuilder MapCreateTranslations(this IEndpointRouteBuilder builder)
    {
        builder.MapPost(ApiEndpoints.Translations.CreateTranslations,
                async (CreateTranslationsRequest request, ISender mediatr, CancellationToken cancellationToken) =>
                {
                    var command = new CreateTranslationsCommand { CreateTranslationsRequest = request };
                    var result = await mediatr.Send(command, cancellationToken);

                    return result.Match(
                        Results.Ok,
                        ResultsHelper.Problem);
                })
            .Produces<CreateTranslationsResponse>()
            .Produces<ApiErrorResponse>(StatusCodes.Status404NotFound)
            .HasApiVersion(1.0)
            .WithName(nameof(ApiEndpoints.Translations.CreateTranslations))
            .AllowAnonymous();

        return builder;
    }
}
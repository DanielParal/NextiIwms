using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Nexticz.Module.Lang.Application.Translations.Commands.UpdateTranslation;
using Nexticz.Module.Lang.Contracts.Translations;
using Nexticz.Lib.Shared.Errors.Models;
using Nexticz.Lib.Shared.Helpers;


namespace Nexticz.Module.Lang.Presentation.Endpoints.Translations;

public static class UpdateTranslationEndpoint
{
    public static IEndpointRouteBuilder MapUpdateTranslation(this IEndpointRouteBuilder builder)
    {
        builder.MapPut(ApiEndpoints.Translations.UpdateTranslation,
                async (Guid id, UpdateTranslationRequest request,
                    ISender mediatr, CancellationToken cancellationToken) =>
                {
                    var command = new UpdateTranslationCommand { UpdateTranslationRequest = request, Id = id };
                    var result = await mediatr.Send(command, cancellationToken);

                    return result.Match(
                        Results.Ok,
                        ResultsHelper.Problem);
                })
            .HasApiVersion(1.0)
            .Produces(StatusCodes.Status204NoContent)
            .Produces<ApiErrorResponse>(StatusCodes.Status422UnprocessableEntity)
            .Produces(StatusCodes.Status404NotFound)
            .WithName(nameof(ApiEndpoints.Translations.UpdateTranslation));

        return builder;
    }
}
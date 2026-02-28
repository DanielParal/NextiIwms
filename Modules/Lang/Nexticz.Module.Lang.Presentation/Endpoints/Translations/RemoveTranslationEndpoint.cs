using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Nexticz.Module.Lang.Application.Translations.Commands.RemoveTranslation;
using Nexticz.Lib.Shared.Helpers;

namespace Nexticz.Module.Lang.Presentation.Endpoints.Translations;

public static class RemoveTranslationEndpoint
{
    public static IEndpointRouteBuilder MapRemoveTranslation(this IEndpointRouteBuilder builder)
    {
        builder.MapDelete(ApiEndpoints.Translations.RemoveTranslation, async (Guid id,
                ISender mediatr, CancellationToken cancellationToken) =>
            {
                var command = new RemoveTranslationCommand { Id = id };
                var result = await mediatr.Send(command, cancellationToken);

                return result.Match(
                    Results.Ok,
                    ResultsHelper.Problem);
            })
            .HasApiVersion(1.0)
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound)
            .WithName(nameof(ApiEndpoints.Translations.RemoveTranslation));

        return builder;
    }
}
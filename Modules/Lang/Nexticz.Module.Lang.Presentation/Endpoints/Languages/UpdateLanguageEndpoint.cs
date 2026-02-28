using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Nexticz.Module.Lang.Application.Languages.Commands.UpdateLanguage;
using Nexticz.Module.Lang.Contracts.Languages;
using Nexticz.Lib.Shared.Errors.Models;
using Nexticz.Lib.Shared.Helpers;


namespace Nexticz.Module.Lang.Presentation.Endpoints.Languages;

public static class UpdateLanguageEndpoint
{
    public static IEndpointRouteBuilder MapUpdateLanguage(this IEndpointRouteBuilder builder)
    {
        builder.MapPut(ApiEndpoints.Languages.UpdateLanguage,
                async (EnumHelper.LanguageShortcutEnum shortcut, UpdateLanguageRequest request,
                    ISender mediatr, CancellationToken cancellationToken) =>
                {
                    var command = new UpdateLanguageCommand { UpdateLanguageRequest = request, Shortcut = shortcut };
                    var result = await mediatr.Send(command, cancellationToken);

                    return result.Match(
                        Results.Ok,
                        ResultsHelper.Problem);
                })
            .Produces(StatusCodes.Status204NoContent)
            .Produces<ApiErrorResponse>(StatusCodes.Status422UnprocessableEntity)
            .Produces<ApiErrorResponse>(StatusCodes.Status404NotFound)
            .HasApiVersion(1.0)
            .WithName(nameof(ApiEndpoints.Languages.UpdateLanguage));

        return builder;
    }
}
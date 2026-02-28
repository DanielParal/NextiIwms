using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Nexticz.Module.Vh.Application.Assortments.Commands.UpdateAssortment;
using Nexticz.Module.Vh.Contracts.Assortments;
using Nexticz.Lib.Shared.Errors.Models;
using Nexticz.Lib.Shared.Helpers;


namespace Nexticz.Module.Vh.Presentation.Endpoints.Assortments;

public static class UpdateAssortmentEndpoint
{
    public static IEndpointRouteBuilder MapUpdateAssortment(this IEndpointRouteBuilder builder)
    {
        builder.MapPut(ApiEndpoints.Assortments.UpdateAssortment,
                async (Guid id, UpdateAssortmentRequest updateAssortmentRequest, ISender mediatr,
                    CancellationToken cancellationToken) =>
                {
                    var command = new UpdateAssortmentCommand { Id = id, UpdateAssortmentRequest = updateAssortmentRequest };
                    var result = await mediatr.Send(command, cancellationToken);

                    return result.Match(
                        _ => Results.NoContent(),
                        ResultsHelper.Problem);
                })
            .Produces(StatusCodes.Status204NoContent)
            .Produces<ApiErrorResponse>(StatusCodes.Status404NotFound)
            .HasApiVersion(1.0)
            .WithName(nameof(ApiEndpoints.Assortments.UpdateAssortment));

        return builder;
    }
}
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Nexticz.Module.Vh.Application.Assortments.Commands.DeleteAssortment;
using Nexticz.Lib.Shared.Errors.Models;
using Nexticz.Lib.Shared.Helpers;


namespace Nexticz.Module.Vh.Presentation.Endpoints.Assortments;

public static class DeleteAssortmentEndpoint
{
    public static IEndpointRouteBuilder MapDeleteAssortment(this IEndpointRouteBuilder builder)
    {
        builder.MapDelete(ApiEndpoints.Assortments.DeleteAssortment,
                async (Guid id, ISender mediatr, CancellationToken cancellationToken) =>
                {
                    var command = new DeleteAssortmentCommand { Id = id };
                    var result = await mediatr.Send(command, cancellationToken);

                    return result.Match(
                        _ => Results.NoContent(),
                        ResultsHelper.Problem);
                })
            .Produces(StatusCodes.Status204NoContent)
            .Produces<ApiErrorResponse>(StatusCodes.Status404NotFound)
            .HasApiVersion(1.0)
            .WithName(nameof(ApiEndpoints.Assortments.DeleteAssortment));

        return builder;
    }
}
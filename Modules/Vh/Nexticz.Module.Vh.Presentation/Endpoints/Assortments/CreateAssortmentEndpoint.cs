using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Nexticz.Module.Vh.Application.Assortments.Commands.CreateAssortment;
using Nexticz.Module.Vh.Contracts.Assortments;
using Nexticz.Lib.Shared.Errors.Models;
using Nexticz.Lib.Shared.Helpers;


namespace Nexticz.Module.Vh.Presentation.Endpoints.Assortments;

public static class CreateAssortmentEndpoint
{
    public static IEndpointRouteBuilder MapCreateAssortment(this IEndpointRouteBuilder builder)
    {
        builder.MapPost(ApiEndpoints.Assortments.CreateAssortment,
                async (CreateAssortmentRequest createAssortmentRequest, ISender mediatr,
                    CancellationToken cancellationToken) =>
                {
                    var command = new CreateAssortmentCommand { CreateAssortmentRequest = createAssortmentRequest };
                    var result = await mediatr.Send(command, cancellationToken);

                    return result.Match(
                        _ => Results.NoContent(),
                        ResultsHelper.Problem);
                })
            .Produces(StatusCodes.Status204NoContent)
            .Produces<ApiErrorResponse>()
            .HasApiVersion(1.0)
            .WithName(nameof(ApiEndpoints.Assortments.CreateAssortment));

        return builder;
    }
}
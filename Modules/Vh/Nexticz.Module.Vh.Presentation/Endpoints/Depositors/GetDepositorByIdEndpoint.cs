using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Nexticz.Module.Vh.Application.Depositors.Queries.GetDepositorById;
using Nexticz.Module.Vh.Contracts.Depositors;
using Nexticz.Lib.Shared.Errors.Models;
using Nexticz.Lib.Shared.Helpers;


namespace Nexticz.Module.Vh.Presentation.Endpoints.Depositors;

public static class GetDepositorByIdEndpoint
{
    public static IEndpointRouteBuilder MapGetDepositorById(this IEndpointRouteBuilder builder)
    {
        builder.MapGet(ApiEndpoints.Depositors.GetDepositorById,
                async (Guid id, ISender mediatr, CancellationToken cancellationToken) =>
                {
                    var query = new GetDepositorByIdQuery { Id = id };
                    var result = await mediatr.Send(query, cancellationToken);

                    return result.Match(
                        Results.Ok,
                        ResultsHelper.Problem);
                })
            .Produces<DepositorResponse>()
            .Produces<ApiErrorResponse>(StatusCodes.Status404NotFound)
            .HasApiVersion(1.0)
            .WithName(nameof(ApiEndpoints.Depositors.GetDepositorById));

        return builder;
    }
}
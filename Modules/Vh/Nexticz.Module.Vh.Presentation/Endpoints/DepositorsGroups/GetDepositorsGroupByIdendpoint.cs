using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Nexticz.Module.Vh.Application.DepositorsGroups.Queries.GetDepositorsGroupByid;
using Nexticz.Module.Vh.Contracts.DepositorsGroups;
using Nexticz.Lib.Shared.Errors.Models;
using Nexticz.Lib.Shared.Helpers;


namespace Nexticz.Module.Vh.Presentation.Endpoints.DepositorsGroups;

public static class GetDepositorsGroupByIdendpoint
{
    public static IEndpointRouteBuilder MapGetDepositorsGroupById(this IEndpointRouteBuilder builder)
    {
        builder.MapGet(ApiEndpoints.DepositorsGroups.GetDepositorsGroupById,
                async (Guid id, ISender mediatr, CancellationToken cancellationToken) =>
                {
                    var query = new GetDepositorsGroupByIdQuery { Id = id };
                    var result = await mediatr.Send(query, cancellationToken);

                    return result.Match(
                        Results.Ok,
                        ResultsHelper.Problem);
                })
            .Produces<DepositorsGroupResponse>()
            .Produces<ApiErrorResponse>(StatusCodes.Status404NotFound)
            .HasApiVersion(1.0)
            .WithName(nameof(ApiEndpoints.DepositorsGroups.GetDepositorsGroupById));
        ;

        return builder;
    }
}
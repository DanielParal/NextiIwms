using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Nexticz.Module.Vh.Application.Depositors.Common.Models;
using Nexticz.Module.Vh.Application.Depositors.Queries.GetDepositors;
using Nexticz.Module.Vh.Contracts.Depositors;
using Nexticz.Lib.Shared.DevExtreme;
using Nexticz.Lib.Shared.Helpers;


namespace Nexticz.Module.Vh.Presentation.Endpoints.Depositors;

public static class GetDepositorsEndpoint
{
    public static IEndpointRouteBuilder MapGetDepositors(this IEndpointRouteBuilder builder)
    {
        builder.MapGet(ApiEndpoints.Depositors.GetDepositors,
                async ([AsParameters] DepositorsFilteringParams filteringParams, ISender mediatr,
                    CancellationToken cancellationToken) =>
                {
                    var query = new GetDepositorsQuery { FilteringParams = filteringParams };
                    var result = await mediatr.Send(query, cancellationToken);
                    return result.Match(
                        Results.Ok,
                        ResultsHelper.Problem);
                })
            .Produces<FilteredResult<DepositorResponse>>()
            .HasApiVersion(1.0)
            .WithName(nameof(ApiEndpoints.Depositors.GetDepositors));

        return builder;
    }
}
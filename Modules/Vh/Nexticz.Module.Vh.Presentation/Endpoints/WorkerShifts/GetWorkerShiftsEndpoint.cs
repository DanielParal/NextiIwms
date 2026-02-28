using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Nexticz.Module.Vh.Application.WorkerShifts.Common.Models;
using Nexticz.Module.Vh.Application.WorkerShifts.Queries.GetWorkerShifts;
using Nexticz.Module.Vh.Contracts.WorkerShifts;
using Nexticz.Module.Vh.Domain.WorkerShifts;
using Nexticz.Module.Vh.Presentation.Endpoints.WorkerShifts.Mappers;
using Nexticz.Lib.Shared.DevExtreme;
using Nexticz.Lib.Shared.Helpers;


namespace Nexticz.Module.Vh.Presentation.Endpoints.WorkerShifts;

public static class GetWorkerShiftsEndpoint
{
    public static IEndpointRouteBuilder MapGetWorkerShifts(this IEndpointRouteBuilder builder)
    {
        builder.MapGet(ApiEndpoints.WorkerShifts.GetWorkerShifts,
                async ([AsParameters] WorkerShiftsFilteringParams filteringParams, ISender mediatr,
                    CancellationToken cancellationToken) =>
                {
                    var query = new GetWorkerShiftsQuery { FilteringParams = filteringParams };
                    var result = await mediatr.Send(query, cancellationToken);

                    if (!result.IsError)
                        result.Value.data = result.Value.data.OfType<WorkerShift>()
                            .Select(x => x.MapToWorkerShiftResponse());

                    return result.Match(
                        Results.Ok,
                        ResultsHelper.Problem);
                })
            .Produces<FilteredResult<WorkerShiftResponse>>()
            .HasApiVersion(1.0)
            .WithName(nameof(ApiEndpoints.WorkerShifts.GetWorkerShifts))
            .AllowAnonymous();

        return builder;
    }
}
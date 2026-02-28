using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Nexticz.Module.Vh.Application.ShiftMasterChanges.Common.Models;
using Nexticz.Module.Vh.Application.ShiftMasterChanges.Queries.GetShiftMasterChanges;
using Nexticz.Module.Vh.Contracts.ShiftMasterChanges;
using Nexticz.Module.Vh.Domain.ShiftMasterChanges;
using Nexticz.Module.Vh.Presentation.Endpoints.ShiftMasterChanges.Mappers;
using Nexticz.Lib.Shared.DevExtreme;
using Nexticz.Lib.Shared.Helpers;


namespace Nexticz.Module.Vh.Presentation.Endpoints.ShiftMasterChanges;

public static class GetShiftMasterChangesEndpoint
{
    public static IEndpointRouteBuilder MapGetShiftMasterChanges(this IEndpointRouteBuilder builder)
    {
        builder.MapGet(ApiEndpoints.ShiftMasterChanges.GetShiftMasterChanges,
                async ([AsParameters] ShiftMasterChangesFilteringParams filteringParams, ISender mediatr,
                    CancellationToken cancellationToken) =>
                {
                    var query = new GetShiftMasterChangesQuery { FilteringParams = filteringParams };
                    var result = await mediatr.Send(query, cancellationToken);

                    if (!result.IsError && result.Value.data.GetType() == typeof(ShiftMasterChange))
                        result.Value.data = result.Value.data.OfType<ShiftMasterChange>()
                            .Select(x => x.MapToShiftMasterChangesResponse());

                    return result.Match(
                        Results.Ok,
                        ResultsHelper.Problem);
                })
            .Produces<FilteredResult<ShiftMasterChangesResponse>>()
            .HasApiVersion(1.0)
            .WithName(nameof(ApiEndpoints.ShiftMasterChanges.GetShiftMasterChanges));

        return builder;
    }
}
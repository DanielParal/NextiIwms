using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Nexticz.Module.Vh.Application.VhUsers.Common.Models;
using Nexticz.Module.Vh.Application.VhUsers.Queries;
using Nexticz.Module.Vh.Contracts.VhUsers;
using Nexticz.Module.Vh.Domain.VhUsers;
using Nexticz.Module.Vh.Presentation.Endpoints.VhUsers.Mappers;
using Nexticz.Lib.Shared.DevExtreme;
using Nexticz.Lib.Shared.Helpers;


namespace Nexticz.Module.Vh.Presentation.Endpoints.VhUsers;

public static class GetVhUsersEndpoint
{
    public static IEndpointRouteBuilder MapGetVhUsers(this IEndpointRouteBuilder builder)
    {
        builder.MapGet(ApiEndpoints.VhUsers.GetVhUsers,
                async ([AsParameters] VhUsersFilteringParams filteringParams, ISender mediatr,
                    CancellationToken cancellationToken) =>
                {
                    var query = new GetVhUsersQuery(filteringParams);
                    var result = await mediatr.Send(query, cancellationToken);

                    if (!result.IsError)
                        result.Value.data = result.Value.data.OfType<VhUser>()
                            .Select(x => x.MapToVhUserResponse());

                    return result.Match(
                        Results.Ok,
                        ResultsHelper.Problem);
                })
            .Produces<FilteredResult<VhUserResponse>>()
            .HasApiVersion(1.0)
            .WithName(nameof(ApiEndpoints.VhUsers.GetVhUsers));

        return builder;
    }
}
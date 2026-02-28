using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Nexticz.Module.Sign.Settings.Contracts.Users;
using Nexticz.Lib.Shared.DevExtreme;
using Nexticz.Module.Sign.Settings.Application.Grouping;
using Nexticz.Module.Sign.Settings.Application.Users;
using Nexticz.Module.Sign.Settings.Application.Users.Queries.GetUsers;
using Nexticz.Module.Sign.Settings.Domain.UserAggregate;

namespace Nexticz.Module.Sign.Settings.Presentation.Users;

internal static class GetUsersEndpoint
{
    public static IEndpointRouteBuilder MapGetUsersEndpoint(this IEndpointRouteBuilder builder)
    {
        builder.MapGet(SettingsEndpoints.UserEndpoints.GetUsers,
                async (
                    [AsParameters] BaseFilteringParams filteringParams,
                    ISender sender,
                    CancellationToken cancellationToken,
                    [FromServices] ISettingsGroupedResultHandler groupedResultHandler
                ) =>
                {
                    if (filteringParams.Group is not null)
                    {
                        return Results.Ok(await groupedResultHandler.GetGroupedResultAsync<User>(filteringParams, cancellationToken));
                    }
                    
                    var result = await sender.Send(new GetUsersQuery(filteringParams), cancellationToken);
                    
                    return Results.Ok(result.MapDataFromTInToTOut(UserResponseFactory.Create));
                })
            .Produces<FilteredResult<UserResponse>>()
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .HasApiVersion(1.0)
            .WithName(SettingsEndpoints.GetOpenApiName(nameof(SettingsEndpoints.UserEndpoints.GetUsers)));

        return builder;
    }
}
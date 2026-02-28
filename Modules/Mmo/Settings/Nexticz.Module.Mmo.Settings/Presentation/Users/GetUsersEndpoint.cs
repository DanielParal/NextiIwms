using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Nexticz.Module.Mmo.Settings.Contracts.Users;
using Nexticz.Lib.Shared.DevExtreme;
using Nexticz.Lib.Shared.Extensions;
using Nexticz.Module.Mmo.Settings.Application.Grouping;
using Nexticz.Module.Mmo.Settings.Application.Users;
using Nexticz.Module.Mmo.Settings.Application.Users.Queries.GetUsers;
using Nexticz.Module.Mmo.Settings.Domain.UserEntity;


namespace Nexticz.Module.Mmo.Settings.Presentation.Users;

internal static class GetUsersEndpoint
{
    public static IEndpointRouteBuilder MapGetUsers(this IEndpointRouteBuilder builder)
    {
        builder.MapGet(SettingsEndpoints.UserEndpoints.GetUsers,
                async (
                    [AsParameters] BaseFilteringParams filteringParams,
                    ISender mediator,
                    CancellationToken cancellationToken,
                    [FromServices] ISettingsGroupedResultHandler groupedResultHandler
                ) =>
                {
                    if (filteringParams.Group is not null)
                    {
                        return Results.Ok(await groupedResultHandler.GetGroupedResultAsync<User>(filteringParams, cancellationToken));
                    }
                    
                    var result = await mediator.Send(new GetUsersQuery(filteringParams), cancellationToken);
                    
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
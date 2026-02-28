using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Nexticz.Module.Vh.Application.DepositorsGroups.Commands.DeleteDepositorsGroup;
using Nexticz.Lib.Shared.Errors.Models;
using Nexticz.Lib.Shared.Helpers;


namespace Nexticz.Module.Vh.Presentation.Endpoints.DepositorsGroups;

public static class DeleteDepositorsGroupEndpoint
{
    public static IEndpointRouteBuilder MapDeleteDepositorsGroup(this IEndpointRouteBuilder builder)
    {
        builder.MapDelete(ApiEndpoints.DepositorsGroups.DeleteDepositorsGroup,
                async (Guid id, ISender mediatr, CancellationToken cancellationToken) =>
                {
                    var command = new DeleteDepositorsGroupCommand { Id = id };
                    var result = await mediatr.Send(command, cancellationToken);

                    return result.Match(
                        _ => Results.NoContent(),
                        ResultsHelper.Problem);
                })
            .Produces(StatusCodes.Status204NoContent)
            .Produces<ApiErrorResponse>(StatusCodes.Status404NotFound)
            .HasApiVersion(1.0)
            .WithName(nameof(ApiEndpoints.DepositorsGroups.DeleteDepositorsGroup));
        ;

        return builder;
    }
}
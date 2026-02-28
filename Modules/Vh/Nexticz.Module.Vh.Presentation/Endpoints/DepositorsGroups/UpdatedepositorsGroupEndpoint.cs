using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Nexticz.Module.Vh.Application.DepositorsGroups.Commands.UpdateDepositorsGroup;
using Nexticz.Module.Vh.Contracts.DepositorsGroups;
using Nexticz.Lib.Shared.Errors.Models;
using Nexticz.Lib.Shared.Helpers;


namespace Nexticz.Module.Vh.Presentation.Endpoints.DepositorsGroups;

public static class UpdatedepositorsGroupEndpoint
{
    public static IEndpointRouteBuilder MapUpdateDepositorsGroup(this IEndpointRouteBuilder builder)
    {
        builder.MapPut(ApiEndpoints.DepositorsGroups.UpdateDepositorsGroup,
                async (Guid id, UpdateDepositorsGroupRequest updateDepositorsGroupRequest, ISender mediatr,
                    CancellationToken cancellationToken) =>
                {
                    var command = new UpdateDepositorsGroupCommand
                        { Id = id, UpdateDepositorsGroupRequest = updateDepositorsGroupRequest };
                    var result = await mediatr.Send(command, cancellationToken);

                    return result.Match(
                        _ => Results.NoContent(),
                        ResultsHelper.Problem);
                })
            .Produces(StatusCodes.Status204NoContent)
            .Produces<ApiErrorResponse>(StatusCodes.Status404NotFound)
            .HasApiVersion(1.0)
            .WithName(nameof(ApiEndpoints.DepositorsGroups.UpdateDepositorsGroup));
        ;

        return builder;
    }
}
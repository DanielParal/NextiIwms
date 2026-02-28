using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Nexticz.Module.Vh.Application.DepositorsGroups.Commands.CreateDepositorsGroup;
using Nexticz.Module.Vh.Contracts.DepositorsGroups;
using Nexticz.Lib.Shared.Errors.Models;
using Nexticz.Lib.Shared.Helpers;


namespace Nexticz.Module.Vh.Presentation.Endpoints.DepositorsGroups;

public static class CreateDepositorsGroupEndpoint
{
    public static IEndpointRouteBuilder MapCreateDepositorsGroup(this IEndpointRouteBuilder builder)
    {
        builder.MapPost(ApiEndpoints.DepositorsGroups.CreateDepositorsGroup,
                async (CreateDepositorsGroupRequest createDepositorsGroupRequest, ISender mediatr,
                    CancellationToken cancellationToken) =>
                {
                    var command = new CreateDepositorsGroupCommand
                        { CreateDepositorsGroupRequest = createDepositorsGroupRequest };
                    var result = await mediatr.Send(command, cancellationToken);

                    return result.Match(
                        _ => Results.NoContent(),
                        ResultsHelper.Problem);
                })
            .Produces(StatusCodes.Status204NoContent)
            .Produces<ApiErrorResponse>()
            .HasApiVersion(1.0)
            .WithName(nameof(ApiEndpoints.DepositorsGroups.CreateDepositorsGroup));

        return builder;
    }
}
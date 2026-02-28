using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Nexticz.Module.Sign.Settings.Contracts.DepositorGroups;
using Nexticz.Lib.Shared.Errors.Models;
using Nexticz.Lib.Shared.Helpers;
using Nexticz.Module.Sign.Settings.Application.DepositorGroups.Commands.CreateDepositorGroup;


namespace Nexticz.Module.Sign.Settings.Presentation.DepositorGroups;

internal static class CreateDepositorGroupEndpoint
{
    public static IEndpointRouteBuilder MapCreateDepositorGroupEndpoint(this IEndpointRouteBuilder builder)
    {
        builder.MapPost(SettingsEndpoints.DepositorGroupEndpoints.CreateDepositorGroup,
                async (
                    CreateDepositorGroupRequest request,
                    ISender sender,
                    CancellationToken cancellationToken) =>
                {
                    var result = 
                        await sender.Send(new CreateDepositorGroupCommand(request.Code, request.Name), cancellationToken);

                    return result.Match(
                        depositorGroup => Results.Created(
                            $"{SettingsEndpoints.DepositorGroupEndpoints.GetDepositorGroups}/{depositorGroup.Code}",
                            DepositorGroupResponseFactory.Create(depositorGroup)),
                        ResultsHelper.Problem);
                })
            .Produces<DepositorGroupResponse>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces<ApiErrorResponse>(StatusCodes.Status422UnprocessableEntity)
            .HasApiVersion(1.0)
            .WithName(SettingsEndpoints.GetOpenApiName(nameof(SettingsEndpoints.DepositorGroupEndpoints.CreateDepositorGroup)));

        return builder;
    }
}
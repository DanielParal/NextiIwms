using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Nexticz.Lib.Shared.Helpers;
using Nexticz.Module.Sign.Settings.Application.Constants.Commands.DeleteConstant;

namespace Nexticz.Module.Sign.Settings.Presentation.Constants;

internal static class DeleteConstantEndpoint
{
    public static IEndpointRouteBuilder MapDeleteConstant(this IEndpointRouteBuilder builder)
    {
        builder.MapDelete(SettingsEndpoints.ConstantEndpoints.DeleteConstant,
                async (
                    string key,
                    ISender mediator, 
                    CancellationToken cancellationToken) =>
                {
                    var command = new DeleteConstantCommand(key);
                    
                    var result = await mediator.Send(command, cancellationToken);

                    return result.Match(
                        _ => Results.NoContent(),
                        ResultsHelper.Problem);
                })
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status404NotFound)
            .HasApiVersion(1.0)
            .WithName(SettingsEndpoints.GetOpenApiName(nameof(SettingsEndpoints.ConstantEndpoints.DeleteConstant)));

        return builder;
    }
}
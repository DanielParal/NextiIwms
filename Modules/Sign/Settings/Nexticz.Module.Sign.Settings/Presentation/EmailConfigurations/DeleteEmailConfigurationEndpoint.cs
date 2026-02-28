using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Nexticz.Lib.Shared.Helpers;
using Nexticz.Module.Sign.Settings.Application.EmailConfigurations.Commands.DeleteEmailConfiguration;

namespace Nexticz.Module.Sign.Settings.Presentation.EmailConfigurations;

internal static class DeleteEmailConfigurationEndpoint
{
    public static IEndpointRouteBuilder MapDeleteEmailConfigurationEndpoint(this IEndpointRouteBuilder builder)
    {
        builder.MapDelete(SettingsEndpoints.EmailConfigurationEndpoints.DeleteEmailConfiguration,
                async (
                    Guid id, 
                    ISender sender,
                    CancellationToken cancellationToken) =>
                {
                    var result = await sender.Send(new DeleteEmailConfigurationCommand(id), cancellationToken);

                    return result.Match(
                        _ => Results.NoContent(),
                        ResultsHelper.Problem);
                })
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status404NotFound)
            .HasApiVersion(1.0)
            .WithName(SettingsEndpoints.GetOpenApiName(nameof(SettingsEndpoints.EmailConfigurationEndpoints.DeleteEmailConfiguration)));

        return builder;
    }
}
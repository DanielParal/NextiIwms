using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Nexticz.Module.Sign.Settings.Contracts.EmailConfigurations;
using Nexticz.Lib.Shared.Errors.Models;
using Nexticz.Lib.Shared.Helpers;
using Nexticz.Module.Sign.Settings.Application.EmailConfigurations.Commands.UpdateEmailConfiguration;

namespace Nexticz.Module.Sign.Settings.Presentation.EmailConfigurations;

internal static class UpdateEmailConfigurationEndpoint
{
    public static IEndpointRouteBuilder MapUpdateEmailConfigurationEndpoint(this IEndpointRouteBuilder builder)
    {
        builder.MapPut(SettingsEndpoints.EmailConfigurationEndpoints.UpdateEmailConfiguration,
                async (
                    Guid id, 
                    UpdateEmailConfigurationRequest request, 
                    ISender sender,
                    CancellationToken cancellationToken) =>
                {
                    var result = 
                        await sender.Send(new UpdateEmailConfigurationCommand(id, request), 
                            cancellationToken);

                    return result.Match(
                        _ => Results.NoContent(),
                        ResultsHelper.Problem);
                })
            .Produces<object>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces<ApiErrorResponse>(StatusCodes.Status422UnprocessableEntity)
            .HasApiVersion(1.0)
            .WithName(SettingsEndpoints.GetOpenApiName(nameof(SettingsEndpoints.EmailConfigurationEndpoints.UpdateEmailConfiguration)));
        
        return builder;
    }
}
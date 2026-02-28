using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Nexticz.Module.Sign.Settings.Contracts.EmailConfigurations;
using Nexticz.Module.Sign.Settings.Domain.EmailConfigurationAggregate;
using Nexticz.Lib.Shared.Errors.Models;
using Nexticz.Lib.Shared.Helpers;
using Nexticz.Module.Sign.Settings.Application.EmailConfigurations.Commands.CreateEmailConfiguration;

namespace Nexticz.Module.Sign.Settings.Presentation.EmailConfigurations;

internal static class CreateEmailConfigurationEndpoint
{
    public static IEndpointRouteBuilder MapCreateEmailConfigurationEndpoint(this IEndpointRouteBuilder builder)
    {
        builder.MapPost(SettingsEndpoints.EmailConfigurationEndpoints.CreateEmailConfiguration,
                async (
                    CreateEmailConfigurationRequest request,
                    ISender sender,
                    CancellationToken cancellationToken) =>
                {
                    var result = 
                        await sender.Send(new CreateEmailConfigurationCommand(request), 
                            cancellationToken);

                    return result.Match(
                        emailConfiguration => Results.Created(
                            $"{SettingsEndpoints.EmailConfigurationEndpoints.GetEmailConfigurations}/{emailConfiguration.Id}",
                            EmailConfigurationResponseFactory.Create(emailConfiguration)),
                        ResultsHelper.Problem);
                })
            .Produces<EmailConfigurationResponse>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces<ApiErrorResponse>(StatusCodes.Status422UnprocessableEntity)
            .HasApiVersion(1.0)
            .WithName(SettingsEndpoints.GetOpenApiName(nameof(SettingsEndpoints.EmailConfigurationEndpoints.CreateEmailConfiguration)));

        return builder;
    }
}
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Nexticz.Module.Sign.Settings.Contracts.EmailConfigurations;
using Nexticz.Lib.Shared.Helpers;
using Nexticz.Module.Sign.Settings.Application.EmailConfigurations.Queries.GetEmailConfigurationById;

namespace Nexticz.Module.Sign.Settings.Presentation.EmailConfigurations;

internal static class GetEmailConfigurationByIdEndpoint
{
    public static IEndpointRouteBuilder MapGetEmailConfigurationByIdEndpoint(this IEndpointRouteBuilder builder)
    {
        builder.MapGet(SettingsEndpoints.EmailConfigurationEndpoints.GetEmailConfigurationById,
                async (
                    Guid id, 
                    ISender sender,
                    CancellationToken cancellationToken
                ) =>
                {
                    var result = 
                        await sender.Send(new GetEmailConfigurationByIdQuery(id), cancellationToken);
                    
                    return result.Match(
                        emailConfiguration => Results.Ok(EmailConfigurationResponseFactory.Create(emailConfiguration)),
                        ResultsHelper.Problem);
                })
            .Produces<EmailConfigurationResponse>()
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status404NotFound)
            .HasApiVersion(1.0)
            .WithName(SettingsEndpoints.GetOpenApiName(nameof(SettingsEndpoints.EmailConfigurationEndpoints.GetEmailConfigurationById)));

        return builder;
    }
}
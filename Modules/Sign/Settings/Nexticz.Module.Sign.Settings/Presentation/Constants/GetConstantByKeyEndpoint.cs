using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Nexticz.Module.Sign.Settings.Contracts.Constants;
using Nexticz.Lib.Shared.Helpers;
using Nexticz.Module.Sign.Settings.Application.Constants;
using Nexticz.Module.Sign.Settings.Application.Constants.Queries.GetConstantByKey;

namespace Nexticz.Module.Sign.Settings.Presentation.Constants;

internal static class GetConstantByKeyEndpoint
{
    public static IEndpointRouteBuilder MapGetConstantByKey(this IEndpointRouteBuilder builder)
    {
        builder.MapGet(SettingsEndpoints.ConstantEndpoints.GetConstantByKey,
                async (
                    string key, 
                    ISender mediator,
                    CancellationToken cancellationToken
                ) =>
                {
                    var request = new GetConstantByKeyQuery(key);
                    var result = await mediator.Send(request, cancellationToken);
                    return result.Match(
                        constant => Results.Ok(ConstantResponseFactory.Create(constant)),
                        ResultsHelper.Problem);
                })
            .Produces<ConstantResponse>()
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status404NotFound)
            .HasApiVersion(1.0)
            .WithName(SettingsEndpoints.GetOpenApiName(nameof(SettingsEndpoints.ConstantEndpoints.GetConstantByKey)));

        return builder;
    }
}
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Nexticz.Module.Auth.Application.Authentications.Queries.GetAccountInfo;
using Nexticz.Module.Auth.Contracts.Authentications;
using Nexticz.Lib.Shared.Errors.Models;
using Nexticz.Lib.Shared.Helpers;


namespace Nexticz.Module.Auth.Presentation.Endpoints.Authentications;

public static class GetAccountInfo
{
    public static IEndpointRouteBuilder MapGetAccountInfo(this IEndpointRouteBuilder builder)
    {
        builder.MapGet(ApiEndpoints.Authentications.GetAccountInfo,
                async ([AsParameters] GetAccountInfoRequest request, ISender mediator,
                    CancellationToken cancellationToken) =>
                {
                    var command = new GetAccountInfoQuery { Username = request.Username };
                    var result = await mediator.Send(command, cancellationToken);

                    return result.Match(
                        Results.Ok,
                        ResultsHelper.Problem);

                }).HasApiVersion(1.0)
            .Produces<GetAccountInfoResponse>()
            .Produces<ApiErrorResponse>(StatusCodes.Status422UnprocessableEntity)
            .WithName(nameof(ApiEndpoints.Authentications.GetAccountInfo));

        return builder;
    }
}
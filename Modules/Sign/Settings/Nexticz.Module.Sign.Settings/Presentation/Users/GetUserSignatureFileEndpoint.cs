using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Nexticz.Module.Sign.Settings.Application.Users.Queries.GetUserSignatureFile;
using Nexticz.Module.Sign.Settings.Contracts.Users.Queries;
using Nexticz.Lib.Shared.Errors.Models;
using Nexticz.Lib.Shared.FileHandling.Models;
using Nexticz.Lib.Shared.Helpers;

namespace Nexticz.Module.Sign.Settings.Presentation.Users;

internal static class GetUserSignatureFileEndpoint
{
    public static IEndpointRouteBuilder MapGetUserSignatureFileEndpoint(this IEndpointRouteBuilder builder)
    {
        builder.MapGet(SettingsEndpoints.UserEndpoints.GetSignature,
                async (
                    string userName,
                    ISender sender,
                    CancellationToken cancellationToken
                ) =>
                {
                    var kitInstructionResult = 
                        await sender.Send(new GetUserSignatureFileQuery(userName), cancellationToken);
                    
                    return kitInstructionResult.Match(
                        userSignature => 
                            Results.Ok(new FileResponse(userSignature.ContentBytes, userSignature.ContentType, userSignature.FileName)),
                        ResultsHelper.Problem);
                })
            .Produces<FileResponse>()
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces<ApiErrorResponse>(StatusCodes.Status422UnprocessableEntity)
            .HasApiVersion(1.0)
            .WithName(SettingsEndpoints.GetOpenApiName(nameof(SettingsEndpoints.UserEndpoints.GetSignature)));

        return builder;
    }
}
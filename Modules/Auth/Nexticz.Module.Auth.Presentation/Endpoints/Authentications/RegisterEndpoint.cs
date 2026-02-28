using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Nexticz.Module.Auth.Application.Authentications.Commands.Register;
using Nexticz.Module.Auth.Application.Common.Interfaces;
using Nexticz.Module.Auth.Contracts.Authentications;
using Nexticz.Lib.Shared.Helpers;

namespace Nexticz.Module.Auth.Presentation.Endpoints.Authentications;

public static class RegisterEndpoint
{
    public static IEndpointRouteBuilder MapRegister(this IEndpointRouteBuilder builder)
    {
        builder.MapPost(ApiEndpoints.Authentications.Register, async (RegisterRequest request, 
                ISender mediatr, CancellationToken cancellationToken, IAuthenticationService authenticationService) =>
            {
                var command = new RegisterCommand { RegisterRequest = request};
                var result = await mediatr.Send(command, cancellationToken);

                if (!result.IsError)
                {
                    authenticationService.AddResponseAuthorizationCookies(result.Value.AccessToken, result.Value.RefreshToken);
                }
                
                return result.Match(
                    Results.Ok, 
                    ResultsHelper.Problem);
        }).HasApiVersion(1.0)
            .Produces<AuthenticationResponse>()
            .WithName(nameof(ApiEndpoints.Authentications.Register));
        
        return builder;
    }
}
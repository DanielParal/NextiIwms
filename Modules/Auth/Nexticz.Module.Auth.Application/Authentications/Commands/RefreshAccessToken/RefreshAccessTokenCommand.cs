using ErrorOr;
using MediatR;
using Nexticz.Module.Auth.Contracts.Authentications;

namespace Nexticz.Module.Auth.Application.Authentications.Commands.RefreshAccessToken;

public class RefreshAccessTokenCommand : IRequest<ErrorOr<AuthenticationResponse>>
{
    public required string AccessToken { get; set; }
    public required string RefreshToken { get; set; }
}
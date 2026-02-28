using ErrorOr;
using MediatR;

namespace Nexticz.Module.Auth.Application.Authentications.Commands.Logout;

public class LogoutCommand : IRequest<ErrorOr<Success>>
{
    public required string AccessToken { get; set; }
    public required string RefreshToken { get; set; }
}
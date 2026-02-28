using ErrorOr;
using MediatR;
using Nexticz.Module.Auth.Contracts.Authentications;

namespace Nexticz.Module.Auth.Application.Authentications.Commands.Register;

public class RegisterCommand : IRequest<ErrorOr<AuthenticationResponse>>
{
    public required RegisterRequest RegisterRequest { get; set; }
}
using ErrorOr;
using MediatR;
using Nexticz.Module.Auth.Contracts.Authentications;

namespace Nexticz.Module.Auth.Application.Authentications.Queries.Login;

public class LoginQuery : IRequest<ErrorOr<AuthenticationResponse>>
{
    public required LoginRequest LoginRequest { get; set; }
}